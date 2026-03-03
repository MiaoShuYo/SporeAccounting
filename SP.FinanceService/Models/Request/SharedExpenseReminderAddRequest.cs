using System.ComponentModel.DataAnnotations;
using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Request;

/// <summary>
/// 分摊提醒新增请求
/// </summary>
public class SharedExpenseReminderAddRequest
{
    /// <summary>
    /// 分摊账目Id
    /// </summary>
    [Required(ErrorMessage = "分摊账目Id不能为空")]
    public long SharedExpenseId { get; set; }

    /// <summary>
    /// 被提醒人Id（参与者）
    /// </summary>
    [Required(ErrorMessage = "被提醒人Id不能为空")]
    public long ParticipantId { get; set; }

    /// <summary>
    /// 提醒类型
    /// </summary>
    [Required(ErrorMessage = "提醒类型不能为空")]
    public ReminderTypeEnum ReminderType { get; set; }

    /// <summary>
    /// 提醒内容
    /// </summary>
    [MaxLength(500, ErrorMessage = "提醒内容长度不能超过500个字")]
    public string? Content { get; set; }

    /// <summary>
    /// 计划提醒时间
    /// </summary>
    [Required(ErrorMessage = "计划提醒时间不能为空")]
    public DateTime ScheduledTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 下次提醒时间（用于重复提醒）
    /// </summary>
    public DateTime? NextReminderTime { get; set; }
}
