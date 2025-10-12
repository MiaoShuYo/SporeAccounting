namespace SP.MLService.Domain
{
    /// <summary>
    /// Learning-to-Rank 训练/预测统一的数据行定义
    /// 
    /// 数据结构说明：
    /// - 每行代表一个查询-候选对，用于LTR算法的训练和预测
    /// - 包含文本特征、类别特征、数值特征和分组信息
    /// - 同一个查询的所有候选项使用相同的GroupId进行分组
    /// 
    /// 使用场景：
    /// - 模型训练：Label=1表示正确匹配，Label=0表示错误匹配
    /// - 模型预测：Label字段在预测时被忽略，重点关注特征
    /// - 数据转换：用户反馈转换为机器学习训练数据的中间格式
    /// 
    /// LTR算法要求：
    /// - GroupId必须相同的记录会被LTR算法视为一组进行排序学习
    /// - 每组内必须至少有一个正样本(Label=1)和一个负样本(Label=0)
    /// - 特征向量的维度和类型必须在训练和预测时保持一致
    /// </summary>
    public sealed class LtrRow
    {
        /// <summary>用户查询文本（消费描述），主要的文本特征，用于语义匹配</summary>
        public string Query { get; set; } = string.Empty;
        
        /// <summary>候选类目名称，另一个文本特征，与查询文本进行匹配</summary>
        public string Candidate { get; set; } = string.Empty;
        
        /// <summary>标签值，1表示正确匹配，0表示错误匹配，预测时可忽略</summary>
        public float Label { get; set; }
        
        /// <summary>分组标识符，同一查询的所有候选项使用相同GroupId，LTR算法必需</summary>
        public string GroupId { get; set; } = string.Empty;

        /// <summary>用户ID，用于个性化推荐的类别特征，可选</summary>
        public string? UserId { get; set; }
        
        /// <summary>商户信息，用于上下文推断的类别特征，可选</summary>
        public string? Merchant { get; set; }
        
        /// <summary>金额桶值，用于金额相关模式识别的数值特征</summary>
        public float AmountBucket { get; set; }
        
        /// <summary>一天中的小时数(0-23)，用于时间模式识别的数值特征</summary>
        public float HourOfDay { get; set; }
    }

    /// <summary>
    /// 模型评分输出（越大越相关）
    /// 
    /// 功能说明：
    /// - ML.NET模型预测的输出格式
    /// - Score值越大表示查询与候选类目的匹配度越高
    /// - 用于排序和置信度判断
    /// 
    /// 数值含义：
    /// - 正值：表示模型认为匹配度较高
    /// - 负值：表示模型认为匹配度较低
    /// - 绝对值大小：表示模型的置信度
    /// 
    /// 使用场景：
    /// - TopK排序：按Score降序排列获得最佳匹配
    /// - 置信度过滤：设定阈值过滤低置信度预测
    /// - 模型评估：分析预测质量和分布
    /// </summary>
    public sealed class LtrScore
    {
        /// <summary>匹配度评分，数值越大表示匹配度越高</summary>
        public float Score { get; set; }
    }

    /// <summary>
    /// 用户自定义类目
    /// 
    /// 数据模型说明：
    /// - 代表用户在记账系统中创建的分类类目
    /// - 支持个性化的类目管理和命名
    /// - 作为机器学习预测的候选项
    /// 
    /// 设计特点：
    /// - 不可变对象，确保数据一致性
    /// - 包含唯一标识符和显示名称
    /// - 支持友好的字符串表示
    /// 
    /// 使用场景：
    /// - 用户创建和管理个人消费分类
    /// - 机器学习模型的预测候选项
    /// - UI界面的类目选择和显示
    /// 
    /// 数据来源：
    /// - 用户手动创建的自定义类目
    /// - 系统预设的通用类目模板
    /// - 从历史数据中学习的推荐类目
    /// </summary>
    public sealed class UserCategory
    {
        /// <summary>
        /// 用户类目构造函数
        /// </summary>
        /// <param name="id">类目唯一标识符，用于数据库存储和引用</param>
        /// <param name="name">类目显示名称，用于用户界面展示和文本匹配</param>
        public UserCategory(string id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>类目唯一标识符，用于数据库存储和系统内部引用</summary>
        public string Id { get; }
        
        /// <summary>类目显示名称，用于用户界面展示和机器学习文本特征</summary>
        public string Name { get; }
        
        /// <summary>
        /// 友好的字符串表示，格式为"名称(ID)"
        /// 便于调试和日志记录
        /// </summary>
        /// <returns>格式化的类目字符串</returns>
        public override string ToString() => $"{Name}({Id})";
    }

    /// <summary>
    /// 类目匹配结果
    /// 
    /// 功能说明：
    /// - 封装机器学习模型的预测结果
    /// - 包含匹配的类目和对应的置信度评分
    /// - 用于排序、过滤和用户界面展示
    /// 
    /// 数据结构：
    /// - Category：匹配的用户类目对象
    /// - Score：模型给出的匹配度评分
    /// 
    /// 使用场景：
    /// - TopK预测结果的返回格式
    /// - 用户界面的候选类目展示
    /// - 置信度阈值过滤和排序
    /// 
    /// 排序规则：
    /// - 通常按Score降序排列
    /// - Score越高表示匹配度越好
    /// - 可用于自动选择或用户确认
    /// </summary>
    public sealed class CategoryMatchResult
    {
        /// <summary>
        /// 类目匹配结果构造函数
        /// </summary>
        /// <param name="category">匹配的用户类目</param>
        /// <param name="score">匹配度评分，数值越大表示匹配度越高</param>
        public CategoryMatchResult(UserCategory category, float score)
        {
            Category = category;
            Score = score;
        }

        /// <summary>匹配的用户类目，包含类目ID和名称信息</summary>
        public UserCategory Category { get; }
        
        /// <summary>匹配度评分，由机器学习模型计算，数值越大表示匹配度越高</summary>
        public float Score { get; }
    }
}


