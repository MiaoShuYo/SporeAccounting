using SP.ReportService.Models.Enumeration;

namespace SP.ReportService.Models.Response;

/// <summary>
/// 预算与实际对比报表响应
/// </summary>
public class BudgetActualComparisonReportResponse
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
    /// 预算总金额
    /// </summary>
    public decimal BudgetedAmount { get; set; }

    /// <summary>
    /// 实际支出金额
    /// </summary>
    public decimal ActualAmount { get; set; }

    /// <summary>
    /// 差异金额
    /// </summary>
    public decimal Variance { get; set; }
}