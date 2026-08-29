namespace SP.ReportService.Models.Response;

/// <summary>
/// 报表智能解读指标响应
/// </summary>
public class ReportInsightMetricResponse
{
    /// <summary>
    /// 指标名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 指标值
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// 指标说明
    /// </summary>
    public string Description { get; set; } = string.Empty;
}