namespace SP.FinanceService.Models.Enumeration;

/// <summary>
/// 分摊提醒状态
/// </summary>
public enum ReminderStatusEnum
{
    /// <summary>
    /// 待发送
    /// </summary>
    Pending = 0,

    /// <summary>
    /// 已发送
    /// </summary>
    Sent = 1,

    /// <summary>
    /// 发送失败
    /// </summary>
    Failed = 2,

    /// <summary>
    /// 已读
    /// </summary>
    Read = 3,

    /// <summary>
    /// 已处理
    /// </summary>
    Resolved = 4
}
