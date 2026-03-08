using SP.ReportService.Models.Enumeration;

namespace SP.ReportService.Models.Response;

/// <summary>
/// 预算消耗趋势报表响应
/// </summary>
public class BudgetConsumptionTrendReportResponse
{
    /// <summary>
    /// 预算周期
    /// </summary>
    public PeriodEnum Period { get; set; }
    
    /// <summary>
    /// 是否综合预算
    /// </summary>
    public bool IsComprehensive { get; set; }
    
    /// <summary>
    /// 收支分类名称
    /// </summary>
    public string CategoryName { get; set; }
    
    /// <summary>
    /// 时间点（如月份、日期、周等）
    /// </summary>
    public string TimePoint { get; set; }

    /// <summary>
    /// 预算消耗金额
    /// </summary>
    public decimal ConsumedAmount { get; set; }
}