using Microsoft.AspNetCore.Mvc;
using Refit;
using SP.FinanceService.Models.Request;

namespace SP.FinanceService.RefitClient;

/// <summary>
/// 站内通知服务API接口
/// </summary>
public interface IInSiteNotificationsServiceApi
{
    /// <summary>
    /// 给某个人发送站内信
    /// </summary>
    /// <param name="sendInSiteNotification">站内信模型</param>
    /// <returns>站内信id</returns>
    [Post("/api/insite-notifications")]
    Task<ApiResponse<long>> SendInSiteNotification([FromBody] SendInSiteNotificationRequest sendInSiteNotification);
}