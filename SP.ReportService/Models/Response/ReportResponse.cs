namespace SP.ReportService.Models.Response;

/// <summary>
/// 报表响应模型
/// </summary>
public class ReportResponse
{
    /// <summary>
    /// 年份
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// 月份
    /// </summary>
    public int Month { get; set; }

    /// <summary>
    /// 金额
    /// </summary>
    public decimal Amount { get; set; }

}