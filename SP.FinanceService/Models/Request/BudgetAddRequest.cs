using System.ComponentModel.DataAnnotations;
using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Request;

/// <summary>
/// 预算添加请求模型
/// </summary>
public class BudgetAddRequest
{
    /// <summary>
    /// 收支类型
    /// </summary>
    [Required(ErrorMessage = "收支类型不能为空")]
    public long TransactionCategoryId { get; set; }

    /// <summary>
    /// 预算金额
    /// </summary>
    [Required(ErrorMessage = "预算金额不能为空")]
    public decimal Amount { get; set; }

    /// <summary>
    /// 预算周期
    /// </summary>
    [Required(ErrorMessage = "预算周期不能为空")]
    public PeriodEnum Period { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(200, ErrorMessage = "备注不能超过200个字符")]
    public string? Remark { get; set; }

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
}