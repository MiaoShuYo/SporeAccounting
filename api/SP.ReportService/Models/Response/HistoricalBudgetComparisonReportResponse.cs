using SP.ReportService.Models.Enumeration;

namespace SP.ReportService.Models.Response;

/// <summary>
/// 历史预算对比报表响应
/// </summary>
public class HistoricalBudgetComparisonReportResponse
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
    /// 时间
    /// </summary>
    public DateTime DateTime { get; set; }

    /// <summary>
    /// 预算金额
    /// </summary>
    public decimal BudgetedAmount { get; set; }
}