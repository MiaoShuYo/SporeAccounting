namespace SP.FinanceService.Score;

/// <summary>
/// 支出分类明细
/// </summary>
public class CategoryExpenseDetail
{
    /// <summary>分类名称</summary>
    public string CategoryName { get; set; } = string.Empty;
    /// <summary>当月支出金额</summary>
    public decimal Amount { get; set; }
    /// <summary>占总支出百分比 (0~1)</summary>
    public decimal Percentage { get; set; }
}