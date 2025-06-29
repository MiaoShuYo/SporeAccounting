namespace SP.FinanceService.Models.Response;

/// <summary>
/// 收支分类响应模型
/// </summary>
public class TransactionCategoryResponse
{
    /// <summary>
    /// 分类ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 分类类型
    /// </summary>
    public int Type { get; set; }
}