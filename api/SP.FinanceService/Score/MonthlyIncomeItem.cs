namespace SP.FinanceService.Score;

/// <summary>
/// 月度收入项
/// </summary>
public class MonthlyIncomeItem
{
    /// <summary>年份</summary>
    public int Year { get; set; }
    /// <summary>月份</summary>
    public int Month { get; set; }
    /// <summary>当月收入金额</summary>
    public decimal Income { get; set; }
}