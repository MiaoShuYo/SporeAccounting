namespace SP.ReportService.Models.Response;

/// <summary>
/// 报表智能解读响应
/// </summary>
public class ReportInsightResponse
{
    /// <summary>
    /// 解读类型
    /// </summary>
    public string InsightType { get; set; } = string.Empty;

    /// <summary>
    /// 数据周期
    /// </summary>
    public string DataPeriod { get; set; } = string.Empty;

    /// <summary>
    /// 概括结论
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// 健康等级（Healthy / Attention / Risk / Overrun / NoData）
    /// </summary>
    public string HealthLevel { get; set; } = string.Empty;

    /// <summary>
    /// 生成方式（Rule / LLM）
    /// </summary>
    public string GeneratedBy { get; set; } = "Rule";

    /// <summary>
    /// 生成时间
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 核心指标
    /// </summary>
    public List<ReportInsightMetricResponse> Metrics { get; set; } = new();

    /// <summary>
    /// 亮点
    /// </summary>
    public List<ReportInsightItemResponse> Highlights { get; set; } = new();

    /// <summary>
    /// 风险提醒
    /// </summary>
    public List<ReportInsightItemResponse> Risks { get; set; } = new();

    /// <summary>
    /// 行动建议
    /// </summary>
    public List<ReportInsightItemResponse> Suggestions { get; set; } = new();
}