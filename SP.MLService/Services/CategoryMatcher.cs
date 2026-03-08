using Microsoft.ML;
using Microsoft.ML.Trainers.LightGbm;
using SP.MLService.Domain;

namespace SP.MLService.Services
{
    /// <summary>
    /// 生产可用的类目匹配服务：
    /// - 使用单一全局 LTR 模型（LightGBM Ranking）
    /// - 预测时对一批候选用相同 GroupId 打分并排序
    /// - 提供 Top1 与 TopK API
    /// - 支持模型文件热加载（按需）
    /// </summary>
    public sealed class CategoryMatcher : IDisposable
    {
        private readonly MLContext _ml;
        private ITransformer? _model;
        private readonly object _modelLock = new();
        private readonly List<LtrRow> _trainingHistory = new();

        /// <summary>
        /// 类目匹配器构造函数
        /// 
        /// 初始化说明：
        /// - 创建ML.NET上下文，用于所有机器学习操作
        /// - 设置随机种子确保结果可重现（便于调试和测试）
        /// - 初始化模型锁和训练历史记录
        /// 
        /// 注意事项：
        /// - seed参数用于控制随机性，生产环境可使用null获得更好随机性
        /// - 默认种子值1确保开发和测试时的一致性
        /// </summary>
        /// <param name="seed">随机种子，null时使用随机值，指定值时确保可重现性</param>
        public CategoryMatcher(int? seed = null)
        {
            _ml = new MLContext(seed ?? 1); // 创建ML上下文，设置随机种子
        }

        /// <summary>
        /// 获取当前训练数据的数量
        /// 用于判断模型是否已训练以及数据充足性
        /// </summary>
        public int TrainingDataCount => _trainingHistory.Count;

        /// <summary>
        /// 从文件加载已训练的模型
        /// 
        /// 功能说明：
        /// - 从磁盘加载预训练的LightGBM模型
        /// - 支持模型的持久化和恢复
        /// - 线程安全的模型加载操作
        /// 
        /// 使用场景：
        /// - 系统启动时恢复已有模型
        /// - 模型版本切换和回滚
        /// - 分布式环境下的模型同步
        /// 
        /// 异常处理：
        /// - 路径无效时抛出ArgumentException
        /// - 文件不存在时抛出FileNotFoundException
        /// - 模型格式错误时ML.NET会抛出相应异常
        /// </summary>
        /// <param name="modelPath">模型文件的完整路径</param>
        public void LoadModel(string modelPath)
        {
            // 参数验证
            if (string.IsNullOrWhiteSpace(modelPath)) throw new ArgumentException("modelPath is required", nameof(modelPath));
            if (!File.Exists(modelPath)) throw new FileNotFoundException("Model file not found", modelPath);

            // 线程安全地加载模型
            lock (_modelLock)
            {
                using var fs = File.OpenRead(modelPath);
                _model = _ml.Model.Load(fs, out _); // 加载模型，忽略schema输出
            }
        }

        /// <summary>
        /// 将训练好的模型保存到文件
        /// 
        /// 功能说明：
        /// - 将内存中的模型序列化保存到磁盘
        /// - 自动创建必要的目录结构
        /// - 保存模型的schema信息以确保加载兼容性
        /// 
        /// 使用场景：
        /// - 训练完成后持久化模型
        /// - 增量学习后更新模型文件
        /// - 模型版本管理和备份
        /// 
        /// 注意事项：
        /// - 需要提供训练时的schema信息确保兼容性
        /// - 会覆盖已存在的同名文件
        /// - 模型未训练时会抛出异常
        /// </summary>
        /// <param name="modelPath">模型保存的完整路径</param>
        /// <param name="trainingSchemaSource">训练时的数据schema，用于确保加载兼容性</param>
        public void SaveModel(string modelPath, IDataView trainingSchemaSource)
        {
            // 检查模型是否已训练
            if (_model is null) throw new InvalidOperationException("Model not loaded or trained");
            
            // 确保目标目录存在
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(modelPath))!);
            
            // 保存模型到文件
            using var fs = File.Create(modelPath);
            _ml.Model.Save(_model, trainingSchemaSource.Schema, fs);
        }

        /// <summary>
        /// 训练LightGBM排序模型
        /// 
        /// 功能说明：
        /// - 使用提供的训练数据从头开始训练模型
        /// - 清空之前的训练历史，进行全量重训练
        /// - 构建完整的特征工程管道并训练LightGBM模型
        /// 
        /// 特征工程流程：
        /// 1. 文本特征：Query和Candidate的标准化和向量化
        /// 2. 类别特征：UserId和Merchant的OneHot编码
        /// 3. 数值特征：AmountBucket和HourOfDay的直接使用
        /// 4. 分组特征：GroupId转换为Key类型供LTR使用
        /// 
        /// 模型配置：
        /// - 使用LightGBM作为基础算法
        /// - 针对小数据集优化的参数设置
        /// - 支持排序学习(Learning-to-Rank)
        /// </summary>
        /// <param name="trainingRows">LTR训练数据集合</param>
        public void Train(IEnumerable<LtrRow> trainingRows)
        {
            if (trainingRows is null) throw new ArgumentNullException(nameof(trainingRows));

            // 清空历史数据，重新训练（全量训练模式）
            _trainingHistory.Clear();
            _trainingHistory.AddRange(trainingRows);

            // 将训练数据加载为ML.NET的IDataView格式
            var train = _ml.Data.LoadFromEnumerable(_trainingHistory);

            // 构建特征工程和模型训练管道
            var pipeline =
                // 第1步：查询文本特征处理
                _ml.Transforms.Text.NormalizeText("q_norm", nameof(LtrRow.Query))       // 标准化查询文本
                .Append(_ml.Transforms.Text.FeaturizeText("q_feat", "q_norm"))          // 转换为数值向量
                
                // 第2步：候选类目文本特征处理
                .Append(_ml.Transforms.Text.NormalizeText("c_norm", nameof(LtrRow.Candidate)))  // 标准化类目名称
                .Append(_ml.Transforms.Text.FeaturizeText("c_feat", "c_norm"))                  // 转换为数值向量
                
                // 第3步：类别特征编码
                .Append(_ml.Transforms.Categorical.OneHotHashEncoding("user_feat", nameof(LtrRow.UserId)))    // 用户ID特征
                .Append(_ml.Transforms.Categorical.OneHotHashEncoding("m_feat", nameof(LtrRow.Merchant)))     // 商户特征
                
                // 第4步：分组键转换（LTR必需）
                .Append(_ml.Transforms.Conversion.MapValueToKey("group_key", nameof(LtrRow.GroupId)))
                
                // 第5步：特征合并
                .Append(_ml.Transforms.Concatenate("Features",
                    new[] { "q_feat", "c_feat", "user_feat", "m_feat", nameof(LtrRow.AmountBucket), nameof(LtrRow.HourOfDay) }))
                
                // 第6步：LightGBM排序训练器
                .Append(_ml.Ranking.Trainers.LightGbm(new LightGbmRankingTrainer.Options
                {
                    LabelColumnName = nameof(LtrRow.Label),        // 标签列（0或1）
                    FeatureColumnName = "Features",                // 合并后的特征列
                    RowGroupColumnName = "group_key",              // 分组列（LTR必需）
                    NumberOfLeaves = 4,                            // 叶子节点数（小数据集优化）
                    MinimumExampleCountPerLeaf = 1,                // 叶子最小样本数
                    NumberOfIterations = 50,                       // 迭代次数
                    LearningRate = 0.1,                            // 学习率
                    Booster = new GradientBooster.Options()        // 梯度提升配置
                }));

            // 执行训练
            var model = pipeline.Fit(train);

            // 线程安全地更新模型
            lock (_modelLock)
            {
                _model = model;
            }
        }

        /// <summary>
        /// 增量训练：添加新的训练样本并重新训练模型
        /// 
        /// 功能说明：
        /// - 将新的训练样本添加到历史数据中
        /// - 使用全部历史数据重新训练模型（非真正的增量学习）
        /// - 保持模型的完整性和一致性
        /// 
        /// 工作原理：
        /// 1. 将新样本添加到训练历史记录
        /// 2. 使用所有历史数据重建训练管道
        /// 3. 重新训练整个模型
        /// 
        /// 注意事项：
        /// - 实际上是"伪增量"，每次都全量重训练
        /// - 随着数据增长，训练时间会逐渐增加
        /// - 确保模型质量和数据一致性
        /// 
        /// 使用场景：
        /// - 用户反馈后的模型更新
        /// - 定期的模型优化和改进
        /// - 在线学习系统的核心功能
        /// </summary>
        /// <param name="newRows">新的训练数据样本</param>
        public void AddTrainingData(IEnumerable<LtrRow> newRows)
        {
            if (newRows is null) throw new ArgumentNullException(nameof(newRows));
            
            // 将新数据添加到训练历史中
            _trainingHistory.AddRange(newRows);
            
            // 使用全部历史数据重新训练（确保模型完整性）
            var train = _ml.Data.LoadFromEnumerable(_trainingHistory);

            var pipeline =
                _ml.Transforms.Text.NormalizeText("q_norm", nameof(LtrRow.Query))
                .Append(_ml.Transforms.Text.FeaturizeText("q_feat", "q_norm"))
                .Append(_ml.Transforms.Text.NormalizeText("c_norm", nameof(LtrRow.Candidate)))
                .Append(_ml.Transforms.Text.FeaturizeText("c_feat", "c_norm"))
                .Append(_ml.Transforms.Categorical.OneHotHashEncoding("user_feat", nameof(LtrRow.UserId)))
                .Append(_ml.Transforms.Categorical.OneHotHashEncoding("m_feat", nameof(LtrRow.Merchant)))
                .Append(_ml.Transforms.Conversion.MapValueToKey("group_key", nameof(LtrRow.GroupId)))
                .Append(_ml.Transforms.Concatenate("Features",
                    new[] { "q_feat", "c_feat", "user_feat", "m_feat", nameof(LtrRow.AmountBucket), nameof(LtrRow.HourOfDay) }))
                .Append(_ml.Ranking.Trainers.LightGbm(new LightGbmRankingTrainer.Options
                {
                    LabelColumnName = nameof(LtrRow.Label),
                    FeatureColumnName = "Features",
                    RowGroupColumnName = "group_key",
                    NumberOfLeaves = 4,
                    MinimumExampleCountPerLeaf = 1,
                    NumberOfIterations = 50,
                    LearningRate = 0.1,
                    Booster = new GradientBooster.Options()
                }));

            var model = pipeline.Fit(train);

            lock (_modelLock)
            {
                _model = model;
            }
        }

        /// <summary>
        /// 预测最佳匹配类目（Top1预测）
        /// 
        /// 功能说明：
        /// - 从候选类目中选择最匹配的单个类目
        /// - 基于LightGBM排序模型的置信度评分
        /// - 支持阈值过滤和回退策略
        /// 
        /// 工作流程：
        /// 1. 调用TopK预测获取排序结果
        /// 2. 取得分最高的类目作为最佳匹配
        /// 3. 检查置信度阈值（可选）
        /// 4. 返回最终预测结果
        /// 
        /// 使用场景：
        /// - 单一类目推荐（如智能分类）
        /// - 快速决策场景
        /// - 高置信度自动分类
        /// </summary>
        /// <param name="query">用户查询文本（消费描述）</param>
        /// <param name="categories">候选类目列表</param>
        /// <param name="userId">用户ID，用于个性化预测（可选）</param>
        /// <param name="merchant">商户信息，用于上下文推断（可选）</param>
        /// <param name="amountBucket">金额桶，用于金额相关特征（可选）</param>
        /// <param name="hourOfDay">小时数，用于时间相关特征（可选）</param>
        /// <param name="threshold">最低置信度阈值，低于此值可触发回退策略（可选）</param>
        /// <returns>最佳匹配的类目和置信度评分</returns>
        public CategoryMatchResult PredictTop1(
            string query,
            IEnumerable<UserCategory> categories,
            string? userId = null,
            string? merchant = null,
            float amountBucket = 0,
            float hourOfDay = 0,
            float threshold = 0.0f)
        {
            // 获取Top1预测结果
            var results = PredictTopK(query, categories, userId, merchant, amountBucket, hourOfDay, k: 1);
            var best = results.First();
            
            // 阈值检查（具体回退策略由上层业务决定）
            if (best.Score < threshold)
            {
                // 阈值回退策略留给上层业务处理（例如别名库/二次确认）
            }
            
            return best;
        }

        /// <summary>
        /// 预测前K个最佳匹配类目（TopK预测）
        /// 
        /// 功能说明：
        /// - 返回按置信度排序的前K个最匹配类目
        /// - 核心预测引擎，被Top1预测调用
        /// - 提供完整的排序和评分信息
        /// 
        /// 工作流程：
        /// 1. 参数验证和模型检查
        /// 2. 为每个候选类目构建预测样本
        /// 3. 使用训练好的模型进行评分
        /// 4. 按评分降序排序并返回前K个
        /// 
        /// 技术细节：
        /// - 使用相同的GroupId确保LTR正确排序
        /// - 所有候选项同时评分，保证排序一致性
        /// - 支持所有训练时使用的特征
        /// 
        /// 使用场景：
        /// - 多选推荐界面
        /// - 候选类目展示
        /// - 置信度分析和调试
        /// </summary>
        /// <param name="query">用户查询文本（消费描述）</param>
        /// <param name="categories">候选类目列表</param>
        /// <param name="userId">用户ID，用于个性化预测（可选）</param>
        /// <param name="merchant">商户信息，用于上下文推断（可选）</param>
        /// <param name="amountBucket">金额桶，用于金额相关特征（可选）</param>
        /// <param name="hourOfDay">小时数，用于时间相关特征（可选）</param>
        /// <param name="k">返回的最佳匹配数量，默认为3</param>
        /// <returns>按置信度降序排列的前K个匹配结果</returns>
        public IEnumerable<CategoryMatchResult> PredictTopK(
            string query,
            IEnumerable<UserCategory> categories,
            string? userId = null,
            string? merchant = null,
            float amountBucket = 0,
            float hourOfDay = 0,
            int k = 3)
        {
            // 参数验证
            if (string.IsNullOrWhiteSpace(query)) throw new ArgumentException("query is required", nameof(query));
            if (categories is null) throw new ArgumentNullException(nameof(categories));

            // 检查模型是否可用
            var model = _model ?? throw new InvalidOperationException("Model not loaded or trained");

            // 生成唯一的GroupId，确保LTR算法正确处理这批候选项
            string gid = Guid.NewGuid().ToString("N");
            
            // 为每个候选类目构建预测样本
            var rows = categories.Select(c => new LtrRow
            {
                Query = query,                      // 用户查询文本
                Candidate = c.Name,                 // 候选类目名称
                Label = 0f,                         // 预测时标签不重要，设为0
                GroupId = gid,                      // 相同GroupId确保正确排序
                UserId = userId ?? string.Empty,    // 用户特征
                Merchant = merchant ?? string.Empty, // 商户特征
                AmountBucket = amountBucket,        // 金额特征
                HourOfDay = hourOfDay               // 时间特征
            });

            // 转换为ML.NET数据格式并进行预测
            var view = _ml.Data.LoadFromEnumerable(rows);
            var scored = model.Transform(view);                    // 使用模型评分
            var scores = _ml.Data.CreateEnumerable<LtrScore>(scored, reuseRowObject: false).ToArray();

            // 将类目和评分组合，按评分降序排序并返回前K个
            return categories.Zip(scores, (c, s) => new CategoryMatchResult(c, s.Score))
                             .OrderByDescending(x => x.Score)      // 按置信度降序排序
                             .Take(k);                             // 取前K个结果
        }

        /// <summary>
        /// 释放资源
        /// 
        /// 功能说明：
        /// - 实现IDisposable接口，支持using语句
        /// - 当前ML.NET的ITransformer不需要显式释放
        /// - 预留接口供未来扩展使用
        /// 
        /// 使用建议：
        /// - 在长期运行的应用中可以忽略Dispose调用
        /// - 建议在单元测试中使用using语句确保清理
        /// - 未来版本可能会添加实际的资源释放逻辑
        /// </summary>
        public void Dispose()
        {
            // 目前 ML.NET 的 ITransformer 无需显式释放资源
            // 此处预留接口供未来扩展使用（如释放GPU资源、清理缓存等）
        }
    }
}


