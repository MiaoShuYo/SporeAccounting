namespace SP.ReportService.Models.Response;

/// <summary>
/// 报表智能解读条目响应
/// </summary>
public class ReportInsightItemResponse
{
    /// <summary>
    /// 维度
    /// </summary>
    public string Dimension { get; set; } = string.Empty;

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 严重程度（Info / Low / Medium / High）
    /// </summary>
    public string Severity { get; set; } = "Info";
}