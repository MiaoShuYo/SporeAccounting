namespace SP.FinanceService.Score;

/// <summary>
/// 预算执行明细
/// </summary>
public class BudgetExecutionDetail
{
    /// <summary>分类名称</summary>
    public string CategoryName { get; set; } = string.Empty;
    /// <summary>预算金额</summary>
    public decimal BudgetAmount { get; set; }
    /// <summary>实际支出</summary>
    public decimal ActualAmount { get; set; }
    /// <summary>超支/节余百分比（正数=超支，负数=节余）</summary>
    public decimal OverrunRate { get; set; }
}