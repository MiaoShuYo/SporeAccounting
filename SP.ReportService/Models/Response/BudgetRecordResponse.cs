using SP.ReportService.Models.Enumeration;

namespace SP.ReportService.Models.Response;

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
    
    /// <summary>
    /// 预算周期
    /// </summary>
    public PeriodEnum Period { get; set; }
    
    /// <summary>
    /// 收支类型id
    /// </summary>
    public long TransactionCategoryId { get; set; }
}