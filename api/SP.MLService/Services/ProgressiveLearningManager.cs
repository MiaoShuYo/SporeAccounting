using SP.MLService.Domain;

namespace SP.MLService.Services
{
    /// <summary>
    /// 渐进式学习管理器：
    /// - 协调模型训练、用户反馈收集、增量学习
    /// - 管理学习阈值和重训策略
    /// - 提供智能预测与回退机制
    /// </summary>
    public sealed class ProgressiveLearningManager : IDisposable
    {
        private readonly CategoryMatcher _matcher;
        private readonly FeedbackStorage _feedbackStorage;
        private readonly string _modelPath;
        private readonly ProgressiveLearningOptions _options;

        /// <summary>
        /// 渐进式学习管理器构造函数（MongoDB版本）
        /// 
        /// 初始化过程：
        /// 1. 验证和保存配置参数
        /// 2. 创建MongoDB反馈存储服务实例
        /// 3. 创建类目匹配器实例
        /// 4. 尝试加载已有的训练模型
        /// 
        /// 注意事项：
        /// - modelPath 目录不存在时会自动创建
        /// - MongoDB数据库和集合不存在时会自动创建
        /// - 自动创建必要的索引以优化查询性能
        /// - 如果模型加载失败，系统会使用规则回退策略
        /// </summary>
        /// <param name="modelPath">ML模型文件保存路径</param>
        /// <param name="connectionString">MongoDB连接字符串</param>
        /// <param name="databaseName">MongoDB数据库名称，默认为"SporeAccountingML"</param>
        /// <param name="collectionName">MongoDB集合名称，默认为"UserFeedbacks"</param>
        /// <param name="options">渐进式学习配置选项，null时使用默认配置</param>
        public ProgressiveLearningManager(
            string modelPath, 
            string connectionString,
            string databaseName = "SporeAccountingML",
            string collectionName = "UserFeedbacks",
            ProgressiveLearningOptions? options = null)
        {
            // 参数验证和赋值
            _modelPath = modelPath ?? throw new ArgumentNullException(nameof(modelPath));
            _feedbackStorage = new FeedbackStorage(connectionString, databaseName, collectionName);
            _matcher = new CategoryMatcher();
            _options = options ?? new ProgressiveLearningOptions();

            // 尝试加载已有的模型（如果存在）
            InitializeModel();
        }

        /// <summary>
        /// 获取当前模型的训练数据量
        /// 用于判断模型是否已经训练以及训练数据的充足性
        /// </summary>
        public int TrainingDataCount => _matcher.TrainingDataCount;
        
        /// <summary>
        /// 获取累积的用户反馈数量
        /// 用于统计用户交互次数和决定是否触发重训练
        /// </summary>
        public int FeedbackCount => _feedbackStorage.Count;

        /// <summary>
        /// 智能预测：结合置信度阈值和回退策略
        /// 
        /// 核心功能：
        /// - 自动选择最佳预测方法（ML模型 vs 规则匹配）
        /// - 基于置信度阈值决定是否需要用户确认
        /// - 提供完整的预测上下文信息（方法、置信度等）
        /// 
        /// 预测流程：
        /// 1. 检查模型可用性和训练数据充足性
        /// 2. 如果条件满足，使用ML模型预测
        /// 3. 如果ML预测失败或置信度低，回退到规则匹配
        /// 4. 返回预测结果和相关元信息
        /// 
        /// 使用场景：
        /// - 用户输入消费描述，系统推荐类目
        /// - 批量导入数据时的自动分类
        /// - 移动端快速记账的智能建议
        /// </summary>
        /// <param name="query">用户查询文本（消费描述）</param>
        /// <param name="categories">用户的自定义类目列表</param>
        /// <param name="userId">用户ID，用于个性化预测（可选）</param>
        /// <param name="merchant">商户信息，用于上下文推断（可选）</param>
        /// <param name="amountBucket">金额桶，用于金额相关的特征（可选）</param>
        /// <param name="hourOfDay">小时数，用于时间相关的特征（可选）</param>
        /// <returns>智能预测结果，包含预测类目、置信度、方法等信息</returns>
        public SmartPredictionResult SmartPredict(
            string query, 
            IEnumerable<UserCategory> categories,
            string userId = "",
            string merchant = "",
            float amountBucket = 0,
            float hourOfDay = 0)
        {
            var categoriesList = categories.ToList();
            
            // 如果模型未训练或类目太少，使用规则回退
            if (_matcher.TrainingDataCount == 0 || categoriesList.Count < 2)
            {
                var fallbackResult = RuleBased_Fallback(query, categoriesList);
                return new SmartPredictionResult
                {
                    PredictedCategory = fallbackResult,
                    Confidence = 0.1f,
                    Method = PredictionMethod.RuleBased,
                    RequiresUserConfirmation = true
                };
            }

            // 使用模型预测
            try
            {
                var result = _matcher.PredictTop1(query, categoriesList, userId, merchant, amountBucket, hourOfDay);
                
                var requiresConfirmation = result.Score < _options.ConfidenceThreshold;
                
                return new SmartPredictionResult
                {
                    PredictedCategory = result.Category,
                    Confidence = result.Score,
                    Method = PredictionMethod.MachineLearning,
                    RequiresUserConfirmation = requiresConfirmation
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ML prediction failed: {ex.Message}, falling back to rules");
                var fallbackResult = RuleBased_Fallback(query, categoriesList);
                return new SmartPredictionResult
                {
                    PredictedCategory = fallbackResult,
                    Confidence = 0.1f,
                    Method = PredictionMethod.RuleBased,
                    RequiresUserConfirmation = true
                };
            }
        }

        /// <summary>
        /// 记录用户选择并触发增量学习
        /// </summary>
        public void RecordUserChoice(
            string query,
            UserCategory selectedCategory,
            IEnumerable<UserCategory> availableCategories,
            string userId = "",
            string merchant = "",
            float amountBucket = 0,
            float hourOfDay = 0)
        {
            var categoryNames = availableCategories.Select(c => c.Name).ToList();
            
            _feedbackStorage.RecordUserChoice(
                query, selectedCategory.Name, userId, merchant, amountBucket, hourOfDay, categoryNames);

            // 检查是否需要重新训练
            TriggerIncrementalLearningIfNeeded();
        }

        /// <summary>
        /// 记录用户纠正并触发增量学习
        /// </summary>
        public void RecordUserCorrection(
            string query,
            UserCategory wrongCategory,
            UserCategory correctCategory,
            IEnumerable<UserCategory> availableCategories,
            string userId = "",
            string merchant = "",
            float amountBucket = 0,
            float hourOfDay = 0)
        {
            var categoryNames = availableCategories.Select(c => c.Name).ToList();
            
            _feedbackStorage.RecordUserCorrection(
                query, wrongCategory.Name, correctCategory.Name, userId, merchant, amountBucket, hourOfDay, categoryNames);

            // 纠正更重要，立即触发重训
            TriggerIncrementalLearning();
        }

        /// <summary>
        /// 手动触发增量学习
        /// </summary>
        public void TriggerIncrementalLearning()
        {
            try
            {
                var trainingData = _feedbackStorage.ToTrainingData(_options.MaxTrainingRecords).ToList();
                
                if (trainingData.Count < _options.MinTrainingDataSize)
                {
                    Console.WriteLine($"Training data insufficient: {trainingData.Count} < {_options.MinTrainingDataSize}");
                    return;
                }

                Console.WriteLine($"Starting incremental learning with {trainingData.Count} samples...");
                
                if (_matcher.TrainingDataCount == 0)
                {
                    // 首次训练
                    _matcher.Train(trainingData);
                }
                else
                {
                    // 增量训练
                    var recentFeedbacks = _feedbackStorage.GetRecentFeedbacks(_options.IncrementalBatchSize);
                    var incrementalData = ConvertFeedbacksToTrainingData(recentFeedbacks).ToList();
                    
                    if (incrementalData.Count > 0)
                    {
                        _matcher.AddTrainingData(incrementalData);
                    }
                }

                // 保存模型
                var schemaSource = MLContextSingleton.GetSchemaFromEnumerable(trainingData);
                _matcher.SaveModel(_modelPath, schemaSource);
                
                Console.WriteLine($"Incremental learning completed. Total training data: {_matcher.TrainingDataCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Incremental learning failed: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取学习统计信息
        /// </summary>
        public LearningStats GetStats()
        {
            return new LearningStats
            {
                TotalFeedbacks = _feedbackStorage.Count,
                TrainingDataSize = _matcher.TrainingDataCount,
                ModelExists = File.Exists(_modelPath),
                RecentFeedbacks = _feedbackStorage.GetRecentFeedbacks(10).Count()
            };
        }

        private void InitializeModel()
        {
            if (File.Exists(_modelPath))
            {
                try
                {
                    _matcher.LoadModel(_modelPath);
                    Console.WriteLine($"Loaded existing model from {_modelPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load model: {ex.Message}");
                }
            }
        }

        private void TriggerIncrementalLearningIfNeeded()
        {
            if (_feedbackStorage.Count % _options.RetrainingFrequency == 0)
            {
                TriggerIncrementalLearning();
            }
        }

        /// <summary>
        /// 规则匹配回退策略
        /// 
        /// 功能说明：
        /// - 当机器学习模型不可用或置信度低时使用的回退方案
        /// - 采用四层递进式匹配策略，从精确到模糊逐级尝试
        /// - 确保在任何情况下都能给出合理的预测结果
        /// 
        /// 四层匹配策略：
        /// 1. 精确关键词匹配：基于预定义规则库的直接匹配
        /// 2. 语义相似度匹配：类目名称和同义词的包含关系匹配
        /// 3. 通用关键词扩展匹配：基于上下文推断的模糊匹配
        /// 4. 智能默认选择：优先选择"其他"类目，最后选择第一个类目
        /// 
        /// 设计理念：
        /// - 宁可保守也不盲目猜测
        /// - 优先选择"其他"类目避免错误分类
        /// - 保持低置信度要求用户确认
        /// </summary>
        /// <param name="query">用户查询文本</param>
        /// <param name="categories">用户的类目列表</param>
        /// <returns>匹配的类目，保证不为null</returns>
        private UserCategory RuleBased_Fallback(string query, List<UserCategory> categories)
        {
            // 将查询转换为小写以进行大小写不敏感的匹配
            var queryLower = query.ToLowerInvariant();
            
            // 预定义的规则库：类目名称 → 关键词列表
            // 这些规则是基于常见消费场景总结的高频关键词
            var rules = new Dictionary<string, string[]>
            {
                // 餐饮：包含各种饮食相关的关键词和知名品牌
                ["餐饮"] = new[] { "餐", "吃", "喝", "茶", "咖啡", "外卖", "饭", "菜", "肯德基", "麦当劳", "星巴克" },
                
                // 交通：涵盖各种出行方式和相关费用
                ["交通"] = new[] { "地铁", "公交", "出租", "滴滴", "车", "票", "油", "停车" },
                
                // 数码：包含电子产品、软件、游戏等科技消费
                ["数码"] = new[] { "手机", "电脑", "充电", "数码", "电子", "软件", "游戏" },
                
                // 娱乐：涵盖各种娱乐休闲活动
                ["娱乐"] = new[] { "电影", "ktv", "游戏", "娱乐", "音乐", "视频", "直播" },
                
                // 购物：包含电商平台和购物场所
                ["购物"] = new[] { "淘宝", "京东", "购物", "买", "商场", "超市" },
                
                // 居家：涵盖家庭生活相关的各种费用
                ["居家"] = new[] { "电费", "水费", "房租", "物业", "家具", "装修" },
                
                // 医疗：包含健康医疗相关的关键词
                ["医疗"] = new[] { "医院", "药", "体检", "看病", "牙科", "医疗" },
                
                // 学习：涵盖教育培训相关的消费
                ["学习"] = new[] { "书", "教育", "培训", "学习", "课程", "学费" }
            };

            // 第1层：精确关键词匹配
            // 遍历用户的类目，检查是否有对应的规则库条目
            foreach (var category in categories)
            {
                if (rules.TryGetValue(category.Name, out var keywords))
                {
                    // 检查查询是否包含该类目的任何关键词
                    if (keywords.Any(k => queryLower.Contains(k)))
                    {
                        return category; // 找到匹配，直接返回
                    }
                }
            }

            // 第2层：语义相似度匹配（基于类目名称和同义词）
            var semanticMatch = FindSemanticMatch(queryLower, categories);
            if (semanticMatch != null)
            {
                return semanticMatch;
            }

            // 第3层：通用关键词扩展匹配（基于上下文推断）
            var generalMatch = FindGeneralMatch(queryLower, categories);
            if (generalMatch != null)
            {
                return generalMatch;
            }

            // 第4层：智能默认选择
            // 优先寻找"其他"、"未分类"等兜底类目，避免错误的强制分类
            var otherCategory = categories.FirstOrDefault(c => 
                c.Name.Contains("其他") || c.Name.Contains("未分类") || c.Name.Contains("杂项"));
            
            // 最终回退：如果没有"其他"类目，选择第一个；如果连类目都没有，创建默认类目
            return otherCategory ?? categories.FirstOrDefault() ?? new UserCategory("unknown", "未知");
        }

        /// <summary>
        /// 基于类目名称的语义相似度匹配
        /// 
        /// 功能说明：
        /// - 检查查询文本与类目名称的直接包含关系
        /// - 使用同义词库进行语义扩展匹配
        /// - 支持双向匹配（查询包含类目 或 类目包含查询）
        /// 
        /// 应用场景：
        /// - "投资股票" → 匹配"投资理财"类目
        /// - "用餐" → 通过同义词匹配"餐饮"类目
        /// - "健康检查" → 通过同义词匹配"医疗"类目
        /// </summary>
        /// <param name="queryLower">已转为小写的查询文本</param>
        /// <param name="categories">用户的类目列表</param>
        /// <returns>匹配的类目，如果没有匹配则返回null</returns>
        private UserCategory? FindSemanticMatch(string queryLower, List<UserCategory> categories)
        {
            // 遍历用户的所有类目进行语义匹配
            foreach (var category in categories)
            {
                var categoryLower = category.Name.ToLowerInvariant();
                
                // 双向包含检查：查询包含类目名 或 类目名包含查询关键词
                // 例如："投资股票" 包含 "投资"，或 "餐饮" 包含 "餐"
                if (queryLower.Contains(categoryLower) || categoryLower.Contains(queryLower))
                {
                    return category;
                }
                
                // 同义词匹配：检查查询是否包含该类目的同义词
                // 例如："用餐" 匹配 "餐饮" 类目的同义词 "饮食"
                var synonyms = GetCategorySynonyms(categoryLower);
                if (synonyms.Any(s => queryLower.Contains(s) || s.Contains(queryLower)))
                {
                    return category;
                }
            }
            
            return null; // 没有找到语义匹配的类目
        }

        /// <summary>
        /// 通用关键词扩展匹配
        /// 
        /// 功能说明：
        /// - 基于上下文关键词推断可能的类目类型
        /// - 处理跨领域的通用词汇匹配
        /// - 提供比精确匹配更宽泛的回退策略
        /// 
        /// 匹配规则：
        /// - 金额相关词汇 → 理财/投资类目
        /// - 服务相关词汇 → 服务/维修类目
        /// - 礼品相关词汇 → 购物/消费类目
        /// 
        /// 应用场景：
        /// - "转账手续费" → 通过"费"+"转账"匹配金融类目
        /// - "汽车保养服务" → 通过"保养"+"服务"匹配服务类目
        /// </summary>
        /// <param name="queryLower">已转为小写的查询文本</param>
        /// <param name="categories">用户的类目列表</param>
        /// <returns>匹配的类目，如果没有匹配则返回null</returns>
        private UserCategory? FindGeneralMatch(string queryLower, List<UserCategory> categories)
        {
            // 金额相关词汇匹配：处理涉及金钱、费用、支付的查询
            // 关键词：钱、元、费、付、收、转账等
            if (queryLower.Contains("钱") || queryLower.Contains("元") || queryLower.Contains("费") || 
                queryLower.Contains("付") || queryLower.Contains("收") || queryLower.Contains("转账"))
            {
                // 在用户类目中寻找金融相关的类目
                var financeCategory = categories.FirstOrDefault(c => 
                    c.Name.Contains("理财") || c.Name.Contains("投资") || c.Name.Contains("金融") || 
                    c.Name.Contains("转账") || c.Name.Contains("支付"));
                if (financeCategory != null) return financeCategory;
            }

            // 服务相关词汇匹配：处理各种服务、维修、保养类查询
            // 关键词：服务、维修、保养等
            if (queryLower.Contains("服务") || queryLower.Contains("维修") || queryLower.Contains("保养"))
            {
                // 在用户类目中寻找服务相关的类目
                var serviceCategory = categories.FirstOrDefault(c => 
                    c.Name.Contains("服务") || c.Name.Contains("维修") || c.Name.Contains("保养"));
                if (serviceCategory != null) return serviceCategory;
            }

            // 礼品购物相关词汇匹配：处理购买、赠送、礼品类查询
            // 关键词：礼、送、买等
            if (queryLower.Contains("礼") || queryLower.Contains("送") || queryLower.Contains("买"))
            {
                // 在用户类目中寻找购物消费相关的类目
                var giftCategory = categories.FirstOrDefault(c => 
                    c.Name.Contains("礼品") || c.Name.Contains("购物") || c.Name.Contains("消费"));
                if (giftCategory != null) return giftCategory;
            }

            return null; // 没有找到通用匹配的类目
        }

        /// <summary>
        /// 获取类目的同义词
        /// 
        /// 功能说明：
        /// - 为每个标准类目名称提供同义词列表
        /// - 扩展语义匹配的覆盖范围
        /// - 支持用户多样化的表达习惯
        /// 
        /// 同义词设计原则：
        /// - 涵盖常见的同义表达
        /// - 包含行业术语和口语化表达
        /// - 考虑地域和文化差异
        /// 
        /// 维护建议：
        /// - 根据用户反馈不断扩展同义词
        /// - 定期分析未匹配的查询补充词库
        /// </summary>
        /// <param name="categoryName">类目名称（小写）</param>
        /// <returns>该类目的同义词数组</returns>
        private string[] GetCategorySynonyms(string categoryName)
        {
            return categoryName switch
            {
                // 餐饮类：涵盖各种饮食相关的表达
                "餐饮" => new[] { "食物", "食品", "饮食", "用餐", "就餐" },
                
                // 交通类：包含出行、通勤等相关词汇
                "交通" => new[] { "出行", "通勤", "运输", "车费", "路费" },
                
                // 数码类：涵盖电子产品、科技设备等
                "数码" => new[] { "电子", "科技", "设备", "电器", "数字" },
                
                // 娱乐类：包含各种休闲娱乐活动
                "娱乐" => new[] { "休闲", "玩乐", "消遣", "放松", "游玩" },
                
                // 购物类：涵盖消费、采购等购买行为
                "购物" => new[] { "消费", "采购", "买东西", "花钱", "支出" },
                
                // 居家类：包含家庭、生活相关的支出
                "居家" => new[] { "家庭", "家用", "生活", "日用", "家居" },
                
                // 医疗类：涵盖健康、治疗等医疗相关
                "医疗" => new[] { "健康", "治疗", "看病", "就医", "医药" },
                
                // 学习类：包含教育、培训等学习相关
                "学习" => new[] { "教育", "培训", "进修", "充电", "提升" },
                
                // 默认情况：没有找到对应类目则返回空数组
                _ => Array.Empty<string>()
            };
        }

        /// <summary>
        /// 将用户反馈转换为机器学习训练数据
        /// 
        /// 功能说明：
        /// - 将用户的选择和纠正行为转换为LTR训练样本
        /// - 为每个反馈生成查询-候选对，正确选择标记为1，其他为0
        /// - 保持GroupId一致性确保LTR算法正确排序学习
        /// 
        /// 数据转换逻辑：
        /// 1. 每个反馈生成一组训练样本（一个查询对应多个候选）
        /// 2. 用户选择的类目标记为正样本（Label=1）
        /// 3. 其他可选类目标记为负样本（Label=0）
        /// 4. 使用相同GroupId确保排序学习的正确性
        /// 
        /// 应用场景：
        /// - 用户选择"餐饮"类目 → 生成"餐饮"(Label=1)和其他类目(Label=0)的训练对
        /// - 用户纠正预测结果 → 生成新的正负样本对用于模型改进
        /// </summary>
        /// <param name="feedbacks">用户反馈数据集合</param>
        /// <returns>转换后的LTR训练数据</returns>
        private IEnumerable<LtrRow> ConvertFeedbacksToTrainingData(IEnumerable<UserFeedback> feedbacks)
        {
            var rows = new List<LtrRow>();
            
            // 遍历每个用户反馈，转换为训练数据
            foreach (var feedback in feedbacks)
            {
                // 使用反馈ID作为GroupId，确保同一次预测的所有候选项使用相同的分组
                var gid = feedback.Id;
                
                // 获取候选类目列表，如果为空则至少包含用户选择的类目
                var candidates = feedback.AvailableCategories.Count > 0 
                    ? feedback.AvailableCategories 
                    : new List<string> { feedback.SelectedCategory };

                // 确保用户选择的类目包含在候选列表中（处理纠正场景）
                if (!candidates.Contains(feedback.SelectedCategory))
                {
                    candidates = new List<string>(candidates) { feedback.SelectedCategory };
                }

                // 为每个候选类目生成一个训练样本
                foreach (var candidate in candidates)
                {
                    // 标签设置：用户选择的类目标记为1（正样本），其他为0（负样本）
                    var label = candidate == feedback.SelectedCategory ? 1f : 0f;
                    
                    // 创建LTR训练行，包含所有特征信息
                    rows.Add(new LtrRow
                    {
                        Query = feedback.Query,           // 用户查询文本
                        Candidate = candidate,            // 候选类目名称
                        Label = label,                    // 标签（1=正确，0=错误）
                        GroupId = gid,                    // 分组ID（同一查询的所有候选共享）
                        UserId = feedback.UserId,         // 用户ID（个性化特征）
                        Merchant = feedback.Merchant,     // 商户信息（上下文特征）
                        AmountBucket = feedback.AmountBucket,  // 金额桶（数值特征）
                        HourOfDay = feedback.HourOfDay    // 时间特征
                    });
                }
            }

            return rows;
        }

        public void Dispose()
        {
            _matcher?.Dispose();
        }
    }

    /// <summary>
    /// 渐进式学习配置选项
    /// </summary>
    public sealed class ProgressiveLearningOptions
    {
        /// <summary>预测置信度阈值，低于此值需要用户确认</summary>
        public float ConfidenceThreshold { get; set; } = 0.35f;
        
        /// <summary>最小训练数据量</summary>
        public int MinTrainingDataSize { get; set; } = 10;
        
        /// <summary>重新训练频率（每N个反馈触发一次）</summary>
        public int RetrainingFrequency { get; set; } = 5;
        
        /// <summary>增量学习批次大小</summary>
        public int IncrementalBatchSize { get; set; } = 10;
        
        /// <summary>单次训练最多加载的反馈记录数，防止全量加载导致 OOM</summary>
        public int MaxTrainingRecords { get; set; } = 50_000;
    }

    /// <summary>
    /// 智能预测结果
    /// </summary>
    public sealed class SmartPredictionResult
    {
        public UserCategory PredictedCategory { get; set; } = new("", "");
        public float Confidence { get; set; }
        public PredictionMethod Method { get; set; }
        public bool RequiresUserConfirmation { get; set; }
    }

    /// <summary>
    /// 学习统计信息
    /// </summary>
    public sealed class LearningStats
    {
        public int TotalFeedbacks { get; set; }
        public int TrainingDataSize { get; set; }
        public bool ModelExists { get; set; }
        public int RecentFeedbacks { get; set; }
    }

    public enum PredictionMethod
    {
        RuleBased,
        MachineLearning
    }
}
