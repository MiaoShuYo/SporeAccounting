namespace SP.MLService.Domain;

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