namespace SP.MLService.Domain;

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