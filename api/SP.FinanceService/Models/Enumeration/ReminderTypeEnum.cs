namespace SP.FinanceService.Models.Enumeration;

/// <summary>
/// 分摊提醒类型
/// </summary>
public enum ReminderTypeEnum
{
    /// <summary>
    /// 未结算提醒
    /// </summary>
    Unpaid = 0,

    /// <summary>
    /// 催款提醒
    /// </summary>
    Overdue = 1,

    /// <summary>
    /// 手动提醒
    /// </summary>
    Manual = 2
}
