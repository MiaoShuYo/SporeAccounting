using System.ComponentModel.DataAnnotations;
using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Request;

/// <summary>
/// 分摊提醒状态更新请求
/// </summary>
public class SharedExpenseReminderStatusUpdateRequest
{
    /// <summary>
    /// 提醒记录Id
    /// </summary>
    [Required(ErrorMessage = "提醒记录Id不能为空")]
    public long Id { get; set; }

    /// <summary>
    /// 提醒状态
    /// </summary>
    [Required(ErrorMessage = "提醒状态不能为空")]
    public ReminderStatusEnum Status { get; set; }
}
