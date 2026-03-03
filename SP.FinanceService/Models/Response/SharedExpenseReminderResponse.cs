using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Response;

/// <summary>
/// 分摊提醒响应
/// </summary>
public class SharedExpenseReminderResponse
{
    /// <summary>
    /// 提醒记录Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 分摊账目Id
    /// </summary>
    public long SharedExpenseId { get; set; }

    /// <summary>
    /// 被提醒人Id
    /// </summary>
    public long ParticipantId { get; set; }

    /// <summary>
    /// 提醒发起人Id
    /// </summary>
    public long ReminderId { get; set; }

    /// <summary>
    /// 提醒类型
    /// </summary>
    public ReminderTypeEnum ReminderType { get; set; }

    /// <summary>
    /// 提醒状态
    /// </summary>
    public ReminderStatusEnum Status { get; set; }

    /// <summary>
    /// 提醒内容
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 计划提醒时间
    /// </summary>
    public DateTime ScheduledTime { get; set; }

    /// <summary>
    /// 实际发送时间
    /// </summary>
    public DateTime? SentTime { get; set; }

    /// <summary>
    /// 下次提醒时间
    /// </summary>
    public DateTime? NextReminderTime { get; set; }
}
