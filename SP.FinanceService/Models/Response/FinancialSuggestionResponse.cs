namespace SP.FinanceService.Models.Response;

/// <summary>
/// 财务改善建议响应模型
/// </summary>
public class FinancialSuggestionResponse
{
    /// <summary>
    /// 评分维度
    /// </summary>
    public string Dimension { get; set; } = string.Empty;

    /// <summary>
    /// 该维度得分
    /// </summary>
    public decimal Score { get; set; }

    /// <summary>
    /// 改善建议
    /// </summary>
    public string Suggestion { get; set; } = string.Empty;

    /// <summary>
    /// 优先级（High / Medium / Low）
    /// </summary>
    public string Priority { get; set; } = string.Empty;
}
