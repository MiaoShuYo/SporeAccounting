using System.ComponentModel.DataAnnotations;

namespace SporeAccounting.Models.ViewModels;

/// <summary>
/// 预算更新视图模型
/// </summary>
public class BudgetUpdateViewModel
{
    /// <summary>
    /// 预算Id
    /// </summary>
    [Required(ErrorMessage = "预算Id不能为空")]
    public string Id { get; set; }

    /// <summary>
    /// 预算金额
    /// </summary>
    [Required(ErrorMessage = "预算金额不能为空")]
    public decimal Amount { get; set; }

    /// <summary>
    /// 周期
    /// </summary>
    [Required(ErrorMessage = "周期不能为空")]
    public PeriodEnum Period { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    [Required(ErrorMessage = "开始时间不能为空")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    [Required(ErrorMessage = "结束时间不能为空")]
    public DateTime EndTime { get; set; }

    /// <summary>
    /// 剩余预算
    /// </summary>
    [Required(ErrorMessage = "剩余预算不能为空")]
    public decimal Remaining { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(200)]
    public string? Remark { get; set; }
}