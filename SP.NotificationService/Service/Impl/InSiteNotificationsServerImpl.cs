using AutoMapper;
using SP.Common;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Model;
using SP.NotificationService.DB;
using SP.NotificationService.Models.Entity;
using SP.NotificationService.Models.Request;
using SP.NotificationService.Models.Response;

namespace SP.NotificationService.Service.Impl;

/// <summary>
/// 站内信实现服务
/// </summary>
public class InSiteNotificationsServerImpl : IInSiteNotificationsServer
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly NotificationServiceDBContext _notificationServiceDb;

    /// <summary>
    /// 自动映射器
    /// </summary>
    private readonly IMapper _autoMapper;

    /// <summary>
    /// 用户id
    /// </summary>
    private readonly long _userId;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="notificationServiceDb"></param>
    /// <param name="autoMapper"></param>
    /// <param name="contextSession"></param>
    public InSiteNotificationsServerImpl(NotificationServiceDBContext notificationServiceDb, IMapper autoMapper,
        ContextSession contextSession)
    {
        _notificationServiceDb = notificationServiceDb;
        _autoMapper = autoMapper;
        _userId = contextSession.UserId;
    }

    /// <summary>
    /// 给指定用户发送站内通知
    /// </summary>
    /// <param name="sendInSiteNotification"></param>
    /// <returns></returns>
    public long SendInSiteNotificationAsync(SendInSiteNotificationRequest sendInSiteNotification)
    {
        InSiteNotification notification = _autoMapper.Map<InSiteNotification>(sendInSiteNotification);
        _notificationServiceDb.InSiteNotifications.Add(notification);
        _notificationServiceDb.SaveChanges();
        return notification.Id;
    }

    /// <summary>
    /// 给全部用户发送站内信
    /// </summary>
    /// <param name="sendInSiteNotification"></param>
    /// <returns></returns>
    public void SendInSiteNotificationToAllUserAsync(SendInSiteNotificationRequest sendInSiteNotification)
    {
        // 这里不要一次性全部发送，可能用户量会很大，应该分批处理
        // 启用后台任务处理
        Task.Run(() =>
        {
            // 分批处理（根据用户量分批）
            var userIds = _notificationServiceDb.InSiteNotifications.Select(x => x.UserId).Distinct().ToList();
            var batchSize = 100;
            for (int i = 0; i < userIds.Count; i += batchSize)
            {
                var batchUserIds = userIds.Skip(i).Take(batchSize).ToList();
                // 批量发送站内信
                BatchSendInSiteNotificationAsync(batchUserIds, sendInSiteNotification.Title,
                    sendInSiteNotification.Content);
            }
        });
    }

    /// <summary>
    /// 标记站内通知为已读
    /// </summary>
    /// <param name="notificationId">通知ID</param>
    /// <returns></returns>
    public void MarkNotificationAsReadAsync(long notificationId)
    {
        var notification = _notificationServiceDb.InSiteNotifications.FirstOrDefault(x => x.Id == notificationId);
        if (notification == null)
        {
            throw new NotFoundException("通知不存在");
        }

        notification.IsRead = true;
        _notificationServiceDb.SaveChanges();
    }

    /// <summary>
    /// 分页获取当前用户的站内通知列表
    /// </summary>
    /// <param name="pageRequest"></param>
    /// <returns></returns>
    public PageResponse<InSiteNotificationRequest> GetUserInSiteNotificationsAsync(
        InSiteNotificationPageRequest pageRequest)
    {
        var notifications = _notificationServiceDb.InSiteNotifications
            .Where(x => x.UserId == _userId && x.IsDeleted == false)
            .OrderByDescending(x => x.CreateDateTime).Skip((pageRequest.PageIndex - 1) * pageRequest.PageSize)
            .Take(pageRequest.PageSize).ToList();
        var totalCount = _notificationServiceDb.InSiteNotifications
            .Where(x => x.UserId == _userId && x.IsDeleted == false).Count();
        return new PageResponse<InSiteNotificationRequest>
        {
            TotalCount = totalCount,
            TotalPage = (int)Math.Ceiling((double)totalCount / pageRequest.PageSize),
            Data = _autoMapper.Map<List<InSiteNotificationRequest>>(notifications),
            PageIndex = pageRequest.PageIndex,
            PageSize = pageRequest.PageSize
        };
    }

    /// <summary>
    /// 获取未读站内通知数量
    /// </summary>
    /// <returns></returns>
    public int GetUnreadNotificationCountAsync()
    {
        return _notificationServiceDb.InSiteNotifications
            .Where(x => x.UserId == _userId && x.IsDeleted == false && x.IsRead == false).Count();
    }

    /// <summary>
    /// 删除站内通知
    /// </summary>
    /// <param name="notificationIds">通知集合</param>
    /// <returns></returns>
    public void DeleteInSiteNotificationsAsync(List<long> notificationIds)
    {
        var notifications = _notificationServiceDb.InSiteNotifications.Where(x => notificationIds.Contains(x.Id))
            .ToList();
        foreach (var notification in notifications)
        {
            SettingCommProperty.Delete(notification);
        }

        _notificationServiceDb.SaveChanges();
    }

    /// <summary>
    /// 修改站内通知
    /// </summary>
    /// <param name="editInSiteNotificationRequest">通知请求</param>
    /// <returns></returns>
    public void EditInSiteNotificationAsync(EditInSiteNotificationRequest editInSiteNotificationRequest)
    {
        var notification =
            _notificationServiceDb.InSiteNotifications.FirstOrDefault(x => x.Id == editInSiteNotificationRequest.Id);
        if (notification == null)
        {
            throw new NotFoundException("通知不存在");
        }

        notification = _autoMapper.Map<InSiteNotification>(editInSiteNotificationRequest);
        SettingCommProperty.Edit(notification);
        _notificationServiceDb.SaveChanges();
    }

    /// <summary>
    /// 获取站内信详情
    /// </summary>
    /// <param name="notificationId">通知ID</param>
    /// <returns></returns>
    public InSiteNotificationRequest GetInSiteNotificationDetailAsync(long notificationId)
    {
        var notification = _notificationServiceDb.InSiteNotifications.FirstOrDefault(x => x.Id == notificationId);
        if (notification == null)
        {
            throw new NotFoundException("通知不存在");
        }
        return _autoMapper.Map<InSiteNotificationRequest>(notification);
    }

    /// <summary>
    /// 批量发送站内信
    /// </summary>
    /// <param name="userIds">用户ID集合</param>
    /// <param name="title">通知标题</param>
    /// <param name="content">通知内容</param>
    /// <returns></returns>
    private void BatchSendInSiteNotificationAsync(List<long> userIds, string title, string content)
    {
        List<InSiteNotification> notifications = new List<InSiteNotification>();
        foreach (var userId in userIds)
        {
            notifications.Add(new InSiteNotification
            {
                UserId = userId,
                Title = title,
                Content = content
            });
        }

        _notificationServiceDb.InSiteNotifications.AddRange(notifications);
        _notificationServiceDb.SaveChanges();
    }
}