namespace SP.FinanceService.Models.Response;

/// <summary>
/// 预算记录响应类
/// </summary>
public class BudgetRecordResponse
{
    /// <summary>
    /// 预算ID
    /// </summary>
    public long BudgetId { get; set; }

    /// <summary>
    /// 记录日期
    /// </summary>
    public DateTime RecordDate { get; set; }

    /// <summary>
    /// 使用金额（正数为消耗，负数为回补）
    /// </summary>
    public decimal UsedAmount { get; set; }
}