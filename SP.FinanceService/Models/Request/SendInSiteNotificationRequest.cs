namespace SP.FinanceService.Models.Request;

/// <summary>
/// 发送站内通知请求模型
/// </summary>
public class SendInSiteNotificationRequest
{
    /// <summary>
    /// 用户ID（为空时表示给所有人发）
    /// </summary>
    public long? UserId { get; set; } = null;
    
    /// <summary>
    /// 通知标题
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// 通知内容
    /// </summary>
    public string Content { get; set; }
}