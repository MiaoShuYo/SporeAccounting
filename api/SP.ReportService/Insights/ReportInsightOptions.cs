namespace SP.ReportService.Insights;

/// <summary>
/// 报表智能解读配置
/// </summary>
public class ReportInsightOptions
{
    /// <summary>
    /// 是否启用 LLM 增强
    /// </summary>
    public bool EnableLlm { get; set; }
}