namespace SP.ReportService.Insights;

/// <summary>
/// 报表分类画像指标
/// </summary>
public class ReportInsightCategoryMetric
{
    /// <summary>
    /// 分类名称
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// 金额
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 占比
    /// </summary>
    public decimal Percentage { get; set; }

    /// <summary>
    /// 预算金额
    /// </summary>
    public decimal BudgetAmount { get; set; }

    /// <summary>
    /// 已用金额
    /// </summary>
    public decimal UsedAmount { get; set; }

    /// <summary>
    /// 剩余金额
    /// </summary>
    public decimal Remaining { get; set; }

    /// <summary>
    /// 使用率
    /// </summary>
    public decimal UsageRate { get; set; }
}