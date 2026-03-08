using AutoMapper;
using SP.NotificationService.Models.Entity;
using SP.NotificationService.Models.Request;
using SP.NotificationService.Models.Response;

namespace SP.NotificationService;

/// <summary>
/// 通知服务映射配置
/// </summary>
public class NotificationProfile:Profile
{
    public NotificationProfile()
    {
        // SendInSiteNotificationRequest to InSiteNotification 映射
        CreateMap<SendInSiteNotificationRequest, InSiteNotification>();
        
        // EditInSiteNotificationRequest to InSiteNotification 映射
        CreateMap<EditInSiteNotificationRequest, InSiteNotification>();
        
        // InSiteNotification to InSiteNotificationRequest 映射
        CreateMap<InSiteNotification, InSiteNotificationRequest>();
    }
}