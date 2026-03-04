using Microsoft.AspNetCore.Mvc;
using SP.Common.Model;
using SP.NotificationService.Models.Request;
using SP.NotificationService.Models.Response;
using SP.NotificationService.Service;

namespace SP.NotificationService.Controllers;

/// <summary>
/// 站内信接口
/// </summary>
[Route("/api/insite-notifications")]
[ApiController]
public class InSiteNotificationsController : ControllerBase
{
    /// <summary>
    /// 站内信服务
    /// </summary>
    private readonly IInSiteNotificationsServer _inSiteNotificationsServer;

    /// <summary>
    /// 站内信控制器构造函数
    /// </summary>
    /// <param name="inSiteNotificationsServer">站内信服务</param>
    public InSiteNotificationsController(IInSiteNotificationsServer inSiteNotificationsServer)
    {
        _inSiteNotificationsServer = inSiteNotificationsServer;
    }

    /// <summary>
    /// 给指定用户发送站内通知
    /// </summary>
    /// <param name="request">站内通知请求</param>
    /// <returns>返回站内通知ID</returns>
    [HttpPost]
    public async Task<ActionResult<long>> SendInSiteNotification([FromBody] SendInSiteNotificationRequest request)
    {
        long notificationId = await _inSiteNotificationsServer.SendInSiteNotificationAsync(request);
        return Ok(notificationId);
    }

    /// <summary>
    /// 给全部用户发送站内通知
    /// </summary>
    /// <param name="request">站内通知请求</param>
    /// <returns>返回站内通知ID</returns>
    [HttpPost("all")]
    public async Task<ActionResult<long>> SendInSiteNotificationToAllUser([FromBody] SendInSiteNotificationRequest request)
    {
        await _inSiteNotificationsServer.SendInSiteNotificationToAllUserAsync(request);
        return Ok();
    }

    /// <summary>
    /// 标记站内通知为已读
    /// </summary>
    /// <param name="notificationId">站内通知ID</param>
    /// <returns>返回标记结果</returns>
    [HttpPut("{notificationId}/read")]
    public async Task<ActionResult<bool>> MarkNotificationAsRead([FromRoute] long notificationId)
    {
        await _inSiteNotificationsServer.MarkNotificationAsReadAsync(notificationId);
        return Ok();
    }

    /// <summary>
    /// 分页获取当前用户的站内通知列表
    /// </summary>
    /// <param name="pageRequest">分页请求</param>
    /// <returns>返回站内通知列表</returns>
    [HttpGet]
    public async Task<ActionResult<PageResponse<InSiteNotificationRequest>>> GetUserInSiteNotifications(
        [FromQuery] InSiteNotificationPageRequest pageRequest)
    {
        var result = await _inSiteNotificationsServer.GetUserInSiteNotificationsAsync(pageRequest);
        return Ok(result);
    }

    /// <summary>
    /// 获取未读站内通知数量
    /// </summary>
    /// <returns>返回未读站内通知数量</returns>
    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadNotificationCount()
    {
        int unreadCount = await _inSiteNotificationsServer.GetUnreadNotificationCountAsync();
        return Ok(unreadCount);
    }

    /// <summary>
    /// 删除站内通知
    /// </summary>
    /// <param name="notificationIds">站内通知ID集合</param>
    /// <returns>返回删除结果</returns>
    [HttpDelete]
    public async Task<ActionResult<bool>> DeleteInSiteNotifications([FromBody] List<long> notificationIds)
    {
        await _inSiteNotificationsServer.DeleteInSiteNotificationsAsync(notificationIds);
        return Ok();
    }

    /// <summary>
    /// 修改站内通知
    /// </summary>
    /// <param name="request">站内通知修改请求</param>
    /// <returns>返回修改结果</returns>
    [HttpPut]
    public async Task<ActionResult<bool>> EditInSiteNotification([FromBody] EditInSiteNotificationRequest request)
    {
        await _inSiteNotificationsServer.EditInSiteNotificationAsync(request);
        return Ok();
    }

    /// <summary>
    /// 获取站内通知详情
    /// </summary>
    /// <param name="notificationId">站内通知ID</param>
    /// <returns>返回站内通知详情</returns>
    [HttpGet("{notificationId}")]
    public async Task<ActionResult<InSiteNotificationRequest>> GetInSiteNotificationDetail([FromRoute] long notificationId)
    {
        var result = await _inSiteNotificationsServer.GetInSiteNotificationDetailAsync(notificationId);
        return Ok(result);
    }
}