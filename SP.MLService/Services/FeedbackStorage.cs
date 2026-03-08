using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SP.MLService.Domain;

namespace SP.MLService.Services
{
    /// <summary>
    /// 用户反馈数据存储服务（MongoDB版本）：
    /// - 记录用户的选择和纠正行为
    /// - 持久化到MongoDB数据库
    /// - 支持批量导出为训练数据
    /// - 提供高性能的查询和统计功能
    /// </summary>
    public sealed class FeedbackStorage
    {
        private readonly IMongoCollection<UserFeedback> _feedbackCollection;
        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly string _collectionName;

        /// <summary>
        /// 用户反馈数据存储服务构造函数（MongoDB版本）
        /// 
        /// 初始化过程：
        /// 1. 验证MongoDB连接参数
        /// 2. 建立MongoDB客户端连接
        /// 3. 获取指定数据库和集合的引用
        /// 4. 创建必要的索引以优化查询性能
        /// 
        /// 注意事项：
        /// - 自动创建数据库和集合（如果不存在）
        /// - 建立时间戳索引以优化时间范围查询
        /// - MongoDB驱动程序自带连接池和线程安全
        /// </summary>
        /// <param name="connectionString">MongoDB连接字符串</param>
        /// <param name="databaseName">数据库名称，默认为"SporeAccountingML"</param>
        /// <param name="collectionName">集合名称，默认为"UserFeedbacks"</param>
        public FeedbackStorage(string connectionString, string databaseName = "SporeAccountingML", string collectionName = "UserFeedbacks")
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
            _collectionName = collectionName ?? throw new ArgumentNullException(nameof(collectionName));

            // 初始化MongoDB连接
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_databaseName);
            _feedbackCollection = database.GetCollection<UserFeedback>(_collectionName);

            // 创建索引以优化查询性能
            CreateIndexes();
        }

        /// <summary>
        /// 获取当前存储的反馈数据总数
        /// 用于统计用户交互次数和判断是否达到重训练阈值
        /// </summary>
        public int Count => (int)_feedbackCollection.CountDocuments(FilterDefinition<UserFeedback>.Empty);

        /// <summary>
        /// 创建MongoDB索引以优化查询性能
        /// 
        /// 索引策略：
        /// - Timestamp降序索引：优化时间范围查询和最新数据获取
        /// - UserId索引：优化用户相关数据查询
        /// - FeedbackType索引：优化按反馈类型过滤
        /// - 复合索引：优化复杂查询场景
        /// </summary>
        private void CreateIndexes()
        {
            try
            {
                var indexKeysDefinition = Builders<UserFeedback>.IndexKeys
                    .Descending(x => x.Timestamp);
                _feedbackCollection.Indexes.CreateOne(new CreateIndexModel<UserFeedback>(indexKeysDefinition));

                var userIndexKeys = Builders<UserFeedback>.IndexKeys
                    .Ascending(x => x.UserId);
                _feedbackCollection.Indexes.CreateOne(new CreateIndexModel<UserFeedback>(userIndexKeys));

                var typeIndexKeys = Builders<UserFeedback>.IndexKeys
                    .Ascending(x => x.FeedbackType);
                _feedbackCollection.Indexes.CreateOne(new CreateIndexModel<UserFeedback>(typeIndexKeys));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to create MongoDB indexes: {ex.Message}");
            }
        }

        /// <summary>
        /// 记录用户选择（正反馈）
        /// 
        /// 功能说明：
        /// - 记录用户主动选择某个类目的行为
        /// - 生成正反馈训练样本，用于模型学习
        /// - 立即持久化到磁盘，防止数据丢失
        /// 
        /// 应用场景：
        /// - 用户确认系统推荐的类目
        /// - 用户从候选列表中主动选择类目
        /// - 批量导入时用户指定的分类规则
        /// 
        /// 数据用途：
        /// - 作为正样本训练机器学习模型
        /// - 统计用户偏好和使用模式
        /// - 触发增量学习和模型更新
        /// </summary>
        /// <param name="query">用户的查询文本（消费描述）</param>
        /// <param name="selectedCategory">用户选择的类目名称</param>
        /// <param name="userId">用户ID，用于个性化学习（可选）</param>
        /// <param name="merchant">商户信息，用于上下文推断（可选）</param>
        /// <param name="amountBucket">金额桶，用于金额相关特征（可选）</param>
        /// <param name="hourOfDay">小时数，用于时间相关特征（可选）</param>
        /// <param name="availableCategories">当时可选的类目列表，用于生成负样本（可选）</param>
        public void RecordUserChoice(string query, string selectedCategory, string userId = "", 
            string merchant = "", float amountBucket = 0, float hourOfDay = 0, 
            IEnumerable<string>? availableCategories = null)
        {
            // 创建用户选择反馈记录
            var feedback = new UserFeedback
            {
                Id = Guid.NewGuid().ToString("N"),        // 生成唯一ID作为训练数据的GroupId
                Query = query,                            // 用户查询文本
                SelectedCategory = selectedCategory,      // 用户选择的正确类目
                UserId = userId,                          // 用户ID（个性化特征）
                Merchant = merchant,                      // 商户信息（上下文特征）
                AmountBucket = amountBucket,              // 金额桶（数值特征）
                HourOfDay = hourOfDay,                    // 时间特征
                AvailableCategories = availableCategories?.ToList() ?? new List<string>(), // 候选类目列表
                Timestamp = DateTime.UtcNow,              // 记录时间戳
                FeedbackType = FeedbackType.UserChoice   // 标记为用户选择类型
            };

            // 异步插入到MongoDB（MongoDB驱动程序线程安全）
            try
            {
                _feedbackCollection.InsertOne(feedback);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to save user choice feedback: {ex.Message}");
                throw; // 重新抛出异常，让上层处理
            }
        }

        /// <summary>
        /// 记录用户纠正（负反馈+正反馈）
        /// 
        /// 功能说明：
        /// - 记录用户纠正系统错误预测的行为
        /// - 同时包含负反馈（错误预测）和正反馈（正确选择）
        /// - 对模型学习具有更高的价值和权重
        /// 
        /// 应用场景：
        /// - 用户不满意系统推荐，手动修改类目
        /// - 用户发现历史分类错误，进行批量纠正
        /// - 系统预测置信度低，用户提供正确答案
        /// 
        /// 数据价值：
        /// - 提供明确的正负样本对比
        /// - 帮助模型学习区分相似但不同的类目
        /// - 优先级高于普通选择，通常立即触发重训练
        /// </summary>
        /// <param name="query">用户的查询文本（消费描述）</param>
        /// <param name="wrongCategory">系统错误预测的类目名称</param>
        /// <param name="correctCategory">用户提供的正确类目名称</param>
        /// <param name="userId">用户ID，用于个性化学习（可选）</param>
        /// <param name="merchant">商户信息，用于上下文推断（可选）</param>
        /// <param name="amountBucket">金额桶，用于金额相关特征（可选）</param>
        /// <param name="hourOfDay">小时数，用于时间相关特征（可选）</param>
        /// <param name="availableCategories">当时可选的类目列表，用于生成完整训练样本（可选）</param>
        public void RecordUserCorrection(string query, string wrongCategory, string correctCategory, 
            string userId = "", string merchant = "", float amountBucket = 0, float hourOfDay = 0,
            IEnumerable<string>? availableCategories = null)
        {
            // 创建用户纠正反馈记录
            var feedback = new UserFeedback
            {
                Id = Guid.NewGuid().ToString("N"),        // 生成唯一ID作为训练数据的GroupId
                Query = query,                            // 用户查询文本
                SelectedCategory = correctCategory,       // 用户提供的正确类目
                WrongCategory = wrongCategory,            // 系统错误预测的类目（重要：用于负样本学习）
                UserId = userId,                          // 用户ID（个性化特征）
                Merchant = merchant,                      // 商户信息（上下文特征）
                AmountBucket = amountBucket,              // 金额桶（数值特征）
                HourOfDay = hourOfDay,                    // 时间特征
                AvailableCategories = availableCategories?.ToList() ?? new List<string>(), // 候选类目列表
                Timestamp = DateTime.UtcNow,              // 记录时间戳
                FeedbackType = FeedbackType.UserCorrection // 标记为用户纠正类型（高价值数据）
            };

            // 插入到MongoDB（纠正数据优先级高，立即保存）
            try
            {
                _feedbackCollection.InsertOne(feedback);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to save user correction feedback: {ex.Message}");
                throw; // 重新抛出异常，让上层处理
            }
        }

        /// <summary>
        /// 转换为 LTR 训练数据（MongoDB版本）
        /// 
        /// 功能说明：
        /// - 从MongoDB查询所有用户反馈并转换为机器学习可用的训练样本
        /// - 每个反馈生成一组查询-候选对（一对多关系）
        /// - 正确类目标记为1，其他候选类目标记为0
        /// 
        /// 转换逻辑：
        /// 1. 从MongoDB获取所有反馈记录
        /// 2. 为每个反馈获取候选类目列表
        /// 3. 确保正确类目包含在候选列表中
        /// 4. 生成标记好的训练样本（正负样本）
        /// 
        /// 性能优化：
        /// - 使用MongoDB的批量查询减少网络开销
        /// - 异常处理确保训练流程不会中断
        /// - 支持大规模数据的高效处理
        /// </summary>
        /// <returns>LTR训练数据集合，每行包含查询、候选、标签和特征</returns>
        public IEnumerable<LtrRow> ToTrainingData(int maxRecords = int.MaxValue)
        {
            var rows = new List<LtrRow>();
            
            try
            {
                // 分页加载反馈记录，按时间戳降序取最新的 maxRecords 条，防止全表扫描导致 OOM
                var feedbacks = _feedbackCollection
                    .Find(FilterDefinition<UserFeedback>.Empty)
                    .SortByDescending(f => f.Timestamp)
                    .Limit(maxRecords == int.MaxValue ? (int?)null : maxRecords)
                    .ToList();
                
                // 遍历所有反馈记录，转换为训练样本
                foreach (var feedback in feedbacks)
                {
                    // 使用反馈ID作为GroupId，确保同一查询的所有候选项归为一组
                    var gid = feedback.Id;
                    
                    // 获取候选类目列表，如果为空则至少包含用户选择的类目
                    var candidates = feedback.AvailableCategories.Count > 0 
                        ? feedback.AvailableCategories 
                        : new List<string> { feedback.SelectedCategory };

                    // 确保用户选择的正确类目包含在候选列表中（处理纠正场景）
                    if (!candidates.Contains(feedback.SelectedCategory))
                    {
                        candidates = new List<string>(candidates) { feedback.SelectedCategory };
                    }

                    // 为每个候选类目生成一个训练样本
                    foreach (var candidate in candidates)
                    {
                        // 标签设置：用户选择的类目为正样本(1)，其他为负样本(0)
                        var label = candidate == feedback.SelectedCategory ? 1f : 0f;
                        
                        // 创建训练行，包含所有特征信息
                        rows.Add(new LtrRow
                        {
                            Query = feedback.Query,           // 查询文本特征
                            Candidate = candidate,            // 候选类目特征
                            Label = label,                    // 训练标签
                            GroupId = gid,                    // 分组ID（LTR必需）
                            UserId = feedback.UserId,         // 用户特征
                            Merchant = feedback.Merchant,     // 商户特征
                            AmountBucket = feedback.AmountBucket, // 金额特征
                            HourOfDay = feedback.HourOfDay    // 时间特征
                        });
                    }
                }

                return rows; // 返回完整的训练数据集
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to load training data from MongoDB: {ex.Message}");
                return new List<LtrRow>(); // 返回空列表，避免中断训练流程
            }
        }

        /// <summary>
        /// 获取最近的反馈（用于增量学习）
        /// 
        /// 功能说明：
        /// - 按时间戳降序返回最近的N个反馈记录
        /// - 主要用于增量学习，避免每次都重新训练全部数据
        /// - 支持滑动窗口式的模型更新策略
        /// 
        /// 使用场景：
        /// - 定期增量更新模型（如每5个反馈触发一次）
        /// - 获取最新用户行为趋势进行分析
        /// - 实现在线学习和实时模型优化
        /// </summary>
        /// <param name="count">要获取的最近反馈数量</param>
        /// <returns>按时间戳降序排列的最近反馈列表</returns>
        public IEnumerable<UserFeedback> GetRecentFeedbacks(int count)
        {
            try
            {
                // 使用MongoDB的排序和限制功能，高效获取最新的count个反馈
                // 利用之前创建的Timestamp降序索引优化查询性能
                return _feedbackCollection
                    .Find(FilterDefinition<UserFeedback>.Empty)
                    .SortByDescending(f => f.Timestamp)
                    .Limit(count)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to get recent feedbacks from MongoDB: {ex.Message}");
                return new List<UserFeedback>(); // 返回空列表，避免中断流程
            }
        }

        /// <summary>
        /// 清空所有反馈数据
        /// 
        /// 功能说明：
        /// - 清空内存中的所有反馈记录
        /// - 同时清空磁盘上的持久化数据
        /// - 不可逆操作，请谨慎使用
        /// 
        /// 使用场景：
        /// - 重置学习系统，从头开始训练
        /// - 清理测试数据，准备生产环境
        /// - 数据迁移或系统维护时的清理操作
        /// 
        /// 注意事项：
        /// - 此操作会导致已训练的模型失效
        /// - 建议在执行前备份重要的反馈数据
        /// </summary>
        public void Clear()
        {
            try
            {
                // 删除MongoDB集合中的所有文档
                _feedbackCollection.DeleteMany(FilterDefinition<UserFeedback>.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to clear feedback data from MongoDB: {ex.Message}");
                throw; // 重新抛出异常，让上层处理
            }
        }

    }

    /// <summary>
    /// 用户反馈记录（MongoDB文档模型）
    /// 
    /// 数据模型说明：
    /// - 记录用户与系统交互的完整信息
    /// - 包含所有用于机器学习的特征数据
    /// - 支持MongoDB的BSON序列化和反序列化
    /// - 优化了索引和查询性能
    /// 
    /// 用途：
    /// - 作为机器学习的训练数据源
    /// - 用户行为分析和统计
    /// - 系统性能评估和优化
    /// 
    /// MongoDB特性：
    /// - 自动生成ObjectId作为主键
    /// - 支持复杂查询和聚合操作
    /// - 提供高可用性和水平扩展能力
    /// </summary>
    public sealed class UserFeedback
    {
        /// <summary>MongoDB文档ID，自动生成</summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? MongoId { get; set; }
        
        /// <summary>唯一标识符，用作训练数据的GroupId</summary>
        [BsonElement("id")]
        public string Id { get; set; } = string.Empty;
        
        /// <summary>用户查询文本（消费描述），主要的文本特征</summary>
        [BsonElement("query")]
        public string Query { get; set; } = string.Empty;
        
        /// <summary>用户选择的正确类目名称，训练时的正样本</summary>
        [BsonElement("selectedCategory")]
        public string SelectedCategory { get; set; } = string.Empty;
        
        /// <summary>系统错误预测的类目名称（仅纠正场景），训练时的负样本</summary>
        [BsonElement("wrongCategory")]
        public string? WrongCategory { get; set; }
        
        /// <summary>用户ID，用于个性化推荐特征</summary>
        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;
        
        /// <summary>商户信息，用于上下文推断特征</summary>
        [BsonElement("merchant")]
        public string Merchant { get; set; } = string.Empty;
        
        /// <summary>金额桶值，用于金额相关的数值特征</summary>
        [BsonElement("amountBucket")]
        public float AmountBucket { get; set; }
        
        /// <summary>一天中的小时数(0-23)，用于时间模式特征</summary>
        [BsonElement("hourOfDay")]
        public float HourOfDay { get; set; }
        
        /// <summary>当时可选的类目列表，用于生成完整的训练样本集</summary>
        [BsonElement("availableCategories")]
        public List<string> AvailableCategories { get; set; } = new();
        
        /// <summary>反馈记录的时间戳，用于排序和时间分析</summary>
        [BsonElement("timestamp")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Timestamp { get; set; }
        
        /// <summary>反馈类型，区分主动选择和错误纠正</summary>
        [BsonElement("feedbackType")]
        [BsonRepresentation(BsonType.String)]
        public FeedbackType FeedbackType { get; set; }
    }

    /// <summary>
    /// 用户反馈类型枚举
    /// 
    /// 类型说明：
    /// - UserChoice: 用户主动选择，表示对系统推荐的确认
    /// - UserCorrection: 用户纠正，表示对系统错误预测的修正
    /// 
    /// 权重差异：
    /// - UserCorrection 通常具有更高的学习价值
    /// - UserCorrection 可能触发立即重训练
    /// - UserChoice 用于常规的模型优化
    /// </summary>
    public enum FeedbackType
    {
        /// <summary>用户主动选择：确认系统推荐或从候选列表中选择</summary>
        UserChoice,
        
        /// <summary>用户纠正错误：修正系统的错误预测，具有更高学习价值</summary>
        UserCorrection
    }
}
