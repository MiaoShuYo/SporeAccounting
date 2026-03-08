using SP.FinanceService.Models.Enumeration;
using System.ComponentModel.DataAnnotations;

namespace SP.FinanceService.Models.Request;

/// <summary>
/// 修改规定开销规则请求
/// </summary>
public class RecurringExpenseRuleEditRequest
{

    /// <summary>
    /// id
    /// </summary>
    [Required(ErrorMessage = "id不能为空")]
    public long Id { get; set; }

    /// <summary>
    /// 账本id
    /// </summary>
    [Required(ErrorMessage = "账本id不能为空")]
    public long AccountBookId { get; set; }
    /// <summary>
    /// 标题
    /// </summary>
    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(20, ErrorMessage = "标题长度不能超过20个字")]
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// 金额
    /// </summary>
    [Required(ErrorMessage = "金额不能为空")]
    public decimal Amount { get; set; }
    /// <summary>
    /// 分类id
    /// </summary>
    [Required(ErrorMessage = "分类id不能为空")]
    public long CategoryId { get; set; }
    /// <summary>
    /// 开始日期
    /// </summary>
    [Required(ErrorMessage = "开始日期不能为空")]
    public DateTime StartDate { get; set; }
    /// <summary>
    /// 结束日期
    /// </summary>
    [Required(ErrorMessage = "结束日期不能为空")]
    public DateTime EndDate { get; set; }
    /// <summary>
    /// 频率（0：天，1：周，2：月，3：季度，4：年）
    /// </summary>
    [Required(ErrorMessage = "频率不能为空")]
    public FrequencyEnum Frequency { get; set; }
}