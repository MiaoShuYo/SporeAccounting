using SP.ReportService.Models.Enumeration;

namespace SP.ReportService.Models.Response;

/// <summary>
/// 预算进度报表响应
/// </summary>
public class BudgetProgressReportResponse
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
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 已使用金额
    /// </summary>
    public decimal UsedAmount { get; set; }

    /// <summary>
    /// 剩余金额
    /// </summary>
    public decimal Remaining { get; set; }
}