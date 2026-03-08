using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SP.Common;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Model;
using SP.NotificationService.DB;
using SP.NotificationService.Models.Entity;
using SP.NotificationService.Models.Request;
using SP.NotificationService.Models.Response;
using SP.NotificationService.RefitClient;

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
        /// 身份服务 Refit 客户端（用于获取全部用户列表）
        /// </summary>
        private readonly IIdentityServiceApi _identityServiceApi;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="notificationServiceDb"></param>
        /// <param name="autoMapper"></param>
        /// <param name="contextSession"></param>
        /// <param name="identityServiceApi"></param>
        public InSiteNotificationsServerImpl(NotificationServiceDBContext notificationServiceDb, IMapper autoMapper,
            ContextSession contextSession, IIdentityServiceApi identityServiceApi)
        {
            _notificationServiceDb = notificationServiceDb;
            _autoMapper = autoMapper;
            _userId = contextSession.UserId;
            _identityServiceApi = identityServiceApi;
        }

    /// <summary>
    /// 给指定用户发送站内通知
    /// </summary>
    /// <param name="sendInSiteNotification"></param>
    /// <returns></returns>
    public async Task<long> SendInSiteNotificationAsync(SendInSiteNotificationRequest sendInSiteNotification)
    {
        if (sendInSiteNotification.UserId == null)
        {
            throw new BusinessException("单个用户通知的 UserId 不能为空，全员发送请使用全员发送接口");
        }

        InSiteNotification notification = _autoMapper.Map<InSiteNotification>(sendInSiteNotification);
        await _notificationServiceDb.InSiteNotifications.AddAsync(notification);
        await _notificationServiceDb.SaveChangesAsync();
        return notification.Id;
    }

    /// 给全部用户发送站内信
    /// </summary>
    /// <param name="sendInSiteNotification"></param>
    /// <returns></returns>
    public async Task SendInSiteNotificationToAllUserAsync(SendInSiteNotificationRequest sendInSiteNotification)
    {
        // 从身份服务获取全部用户 ID，分页拉取避免内存渢出
        const int pageSize = 100;
        var pageIndex = 1;
        var allUserIds = new List<long>();

        while (true)
        {
            var response = await _identityServiceApi.GetUsers(
                new UserPageRequest { PageIndex = pageIndex, PageSize = pageSize });

            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                throw new BusinessException("获取用户列表失败，无法发送站内信广播");
            }

            var page = response.Content;
            allUserIds.AddRange(page.Data.Select(u => u.Id));

            if (pageIndex >= page.TotalPage)
                break;

            pageIndex++;
        }

        // 分批发送站内信
        const int batchSize = 100;
        for (int i = 0; i < allUserIds.Count; i += batchSize)
        {
            var batchUserIds = allUserIds.Skip(i).Take(batchSize).ToList();
            await BatchSendInSiteNotificationAsync(batchUserIds, sendInSiteNotification.Title,
                sendInSiteNotification.Content);
        }
    }

    /// <summary>
    /// 标记站内通知为已读
    /// </summary>
    /// <param name="notificationId">通知ID</param>
    /// <returns></returns>
    public async Task MarkNotificationAsReadAsync(long notificationId)
    {
        var notification = await _notificationServiceDb.InSiteNotifications
            .FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == _userId && !x.IsDeleted);
        if (notification == null)
        {
            throw new NotFoundException("通知不存在");
        }

        notification.IsRead = true;
        await _notificationServiceDb.SaveChangesAsync();
    }

    /// <summary>
    /// 分页获取当前用户的站内通知列表
    /// </summary>
    /// <param name="pageRequest"></param>
    /// <returns></returns>
    public async Task<PageResponse<InSiteNotificationRequest>> GetUserInSiteNotificationsAsync(
        InSiteNotificationPageRequest pageRequest)
    {
        var notifications = await _notificationServiceDb.InSiteNotifications
            .Where(x => x.UserId == _userId && x.IsDeleted == false)
            .OrderByDescending(x => x.CreateDateTime).Skip((pageRequest.PageIndex - 1) * pageRequest.PageSize)
            .Take(pageRequest.PageSize)
            .ToListAsync();
        var totalCount = await _notificationServiceDb.InSiteNotifications
            .Where(x => x.UserId == _userId && x.IsDeleted == false)
            .CountAsync();
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
    public async Task<int> GetUnreadNotificationCountAsync()
    {
        return await _notificationServiceDb.InSiteNotifications
            .Where(x => x.UserId == _userId && x.IsDeleted == false && x.IsRead == false)
            .CountAsync();
    }

    /// <summary>
    /// 删除站内通知
    /// </summary>
    /// <param name="notificationIds">通知集合</param>
    /// <returns></returns>
    public async Task DeleteInSiteNotificationsAsync(List<long> notificationIds)
    {
        var notifications = await _notificationServiceDb.InSiteNotifications
            .Where(x => notificationIds.Contains(x.Id) && x.UserId == _userId && !x.IsDeleted)
            .ToListAsync();
        foreach (var notification in notifications)
        {
            SettingCommProperty.Delete(notification);
        }

        await _notificationServiceDb.SaveChangesAsync();
    }

    /// <summary>
    /// 修改站内通知
    /// </summary>
    /// <param name="editInSiteNotificationRequest">通知请求</param>
    /// <returns></returns>
    public async Task EditInSiteNotificationAsync(EditInSiteNotificationRequest editInSiteNotificationRequest)
    {
        var notification = await _notificationServiceDb.InSiteNotifications
            .FirstOrDefaultAsync(x => x.Id == editInSiteNotificationRequest.Id && x.UserId == _userId && !x.IsDeleted);
        if (notification == null)
        {
            throw new NotFoundException("通知不存在");
        }

        // 映射到已跟踪实体，避免替换对象导致更新不落库
        _autoMapper.Map(editInSiteNotificationRequest, notification);
        SettingCommProperty.Edit(notification);
        await _notificationServiceDb.SaveChangesAsync();
    }

    /// <summary>
    /// 获取站内信详情
    /// </summary>
    /// <param name="notificationId">通知ID</param>
    /// <returns></returns>
    public async Task<InSiteNotificationRequest> GetInSiteNotificationDetailAsync(long notificationId)
    {
        var notification = await _notificationServiceDb.InSiteNotifications
            .FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == _userId && !x.IsDeleted);
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
    private async Task BatchSendInSiteNotificationAsync(List<long> userIds, string title, string content)
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

        await _notificationServiceDb.InSiteNotifications.AddRangeAsync(notifications);
        await _notificationServiceDb.SaveChangesAsync();
    }
}