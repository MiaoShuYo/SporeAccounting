using SP.FinanceService.Models.Enumeration;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service;

/// <summary>
/// 分摊提醒服务接口
/// </summary>
public interface ISharedExpenseReminderServer
{
    /// <summary>
    /// 新增分摊提醒
    /// </summary>
    /// <param name="request">提醒请求</param>
    /// <returns>提醒记录Id</returns>
    long Add(SharedExpenseReminderAddRequest request);

    /// <summary>
    /// 根据分摊账目Id查询提醒记录
    /// </summary>
    /// <param name="sharedExpenseId">分摊账目Id</param>
    /// <returns>提醒记录列表</returns>
    List<SharedExpenseReminderResponse> QueryBySharedExpenseId(long sharedExpenseId);

    /// <summary>
    /// 查询待发送提醒
    /// </summary>
    /// <param name="now">当前时间</param>
    /// <returns>提醒记录列表</returns>
    List<SharedExpenseReminderResponse> QueryPending(DateTime now);

    /// <summary>
    /// 更新提醒状态
    /// </summary>
    /// <param name="id">提醒记录Id</param>
    /// <param name="status">提醒状态</param>
    /// <param name="sentTime">发送时间</param>
    void UpdateStatus(long id, ReminderStatusEnum status, DateTime? sentTime = null);

    /// <summary>
    /// 重新安排提醒
    /// </summary>
    /// <param name="id">提醒记录Id</param>
    /// <param name="status">提醒状态</param>
    /// <param name="scheduledTime">下次计划时间</param>
    /// <param name="sentTime">发送时间</param>
    /// <param name="nextReminderTime">下次重复提醒时间</param>
    void Reschedule(long id, ReminderStatusEnum status, DateTime scheduledTime, DateTime? sentTime, DateTime? nextReminderTime);
}
