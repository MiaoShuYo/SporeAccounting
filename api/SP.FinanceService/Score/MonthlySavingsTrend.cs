namespace SP.FinanceService.Score;

/// <summary>
/// 月度储蓄率趋势点
/// </summary>
public class MonthlySavingsTrend
{
    /// <summary>年份</summary>
    public int Year { get; set; }
    /// <summary>月份</summary>
    public int Month { get; set; }
    /// <summary>储蓄率 (0~1)</summary>
    public decimal SavingsRate { get; set; }
}