namespace SP.MLService.Domain;

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