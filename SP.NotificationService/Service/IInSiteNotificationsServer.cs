using SP.Common.Model;
using SP.NotificationService.Models.Entity;
using SP.NotificationService.Models.Request;
using SP.NotificationService.Models.Response;

namespace SP.NotificationService.Service;

/// <summary>
/// 站内信服务接口
/// </summary>
public interface IInSiteNotificationsServer
{
    /// <summary>
    /// 给指定用户发送站内通知
    /// </summary>
    /// <param name="sendInSiteNotification"></param>
    /// <returns></returns>
    void SendInSiteNotificationAsync(SendInSiteNotificationRequest sendInSiteNotification);

    /// <summary>
    /// 给全部用户发送站内信
    /// </summary>
    /// <param name="sendInSiteNotification"></param>
    /// <returns></returns>
    void SendInSiteNotificationToAllUserAsync(SendInSiteNotificationRequest sendInSiteNotification);

    /// <summary>
    /// 标记站内通知为已读
    /// </summary>
    /// <param name="notificationId">通知ID</param>
    /// <returns></returns>
    void MarkNotificationAsReadAsync(long notificationId);

    /// <summary>
    /// 分页获取当前用户的站内通知列表
    /// </summary>
    /// <param name="pageRequest"></param>
    /// <returns></returns>
    PageResponse<InSiteNotificationRequest> GetUserInSiteNotificationsAsync(
        InSiteNotificationPageRequest pageRequest);

    /// <summary>
    /// 获取未读站内通知数量
    /// </summary>
    /// <returns></returns>
    int GetUnreadNotificationCountAsync();

    /// <summary>
    /// 删除站内通知
    /// </summary>
    /// <param name="notificationIds">通知集合</param>
    /// <returns></returns>
    void DeleteInSiteNotificationsAsync(List<long> notificationIds);

    /// <summary>
    /// 修改站内通知
    /// </summary>
    /// <param name="editInSiteNotificationRequest">通知请求</param>
    /// <returns></returns>
    void EditInSiteNotificationAsync(EditInSiteNotificationRequest editInSiteNotificationRequest);

    /// <summary>
    /// 获取站内信详情
    /// </summary>
    /// <param name="notificationId">通知ID</param>
    /// <returns></returns>
    InSiteNotificationRequest GetInSiteNotificationDetailAsync(long notificationId);
}