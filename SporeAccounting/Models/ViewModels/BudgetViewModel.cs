namespace SporeAccounting.Models.ViewModels;
/// <summary>
/// 预算视图模型
/// </summary>
public class BudgetViewModel
{
    /// <summary>
    /// 预算Id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 预算金额
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 周期
    /// </summary>
    public PeriodEnum Period { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// 剩余预算
    /// </summary>
    public decimal Remaining { get; set; }

    /// <summary>
    /// 收支分类
    /// </summary>
    public string ClassificationName { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}