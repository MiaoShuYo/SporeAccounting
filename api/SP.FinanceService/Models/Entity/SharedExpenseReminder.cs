using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Common.Model;
using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Entity;

/// <summary>
/// 分摊提醒记录实体
/// </summary>
[Table("SharedExpenseReminder")]
public class SharedExpenseReminder : BaseModel
{
    /// <summary>
    /// 分摊账目Id
    /// </summary>
    [Column(TypeName = "bigint")]
    [Required]
    public long SharedExpenseId { get; set; }

    /// <summary>
    /// 被提醒人Id（参与者）
    /// </summary>
    [Column(TypeName = "bigint")]
    [Required]
    public long ParticipantId { get; set; }

    /// <summary>
    /// 提醒发起人Id（通常是垫付人/发起人）
    /// </summary>
    [Column(TypeName = "bigint")]
    [Required]
    public long ReminderId { get; set; }

    /// <summary>
    /// 提醒类型
    /// </summary>
    [Column(TypeName = "tinyint")]
    [Required]
    public ReminderTypeEnum ReminderType { get; set; }

    /// <summary>
    /// 提醒状态
    /// </summary>
    [Column(TypeName = "tinyint")]
    [Required]
    public ReminderStatusEnum Status { get; set; } = ReminderStatusEnum.Pending;

    /// <summary>
    /// 提醒内容
    /// </summary>
    [Column(TypeName = "nvarchar(500)")]
    public string? Content { get; set; }

    /// <summary>
    /// 计划提醒时间
    /// </summary>
    [Column(TypeName = "datetime")]
    [Required]
    public DateTime ScheduledTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 实际发送时间
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? SentTime { get; set; }

    /// <summary>
    /// 下次提醒时间（用于重复提醒）
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? NextReminderTime { get; set; }
}