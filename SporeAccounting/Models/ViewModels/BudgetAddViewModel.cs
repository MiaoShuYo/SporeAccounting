using System.ComponentModel.DataAnnotations;

namespace SporeAccounting.Models.ViewModels;

/// <summary>
/// 预算添加视图模型
/// </summary>
public class BudgetAddViewModel
{
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
    /// 收支分类
    /// </summary>
    [Required(ErrorMessage = "收支分类不能为空")]
    public string IncomeExpenditureClassificationId { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(200)]
    public string? Remark { get; set; }
}