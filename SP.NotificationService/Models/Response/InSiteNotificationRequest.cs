namespace SP.NotificationService.Models.Response;

/// <summary>
/// 站内通知响应模型
/// </summary>
public class InSiteNotificationRequest
{
    /// <summary>
    /// 通知ID
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// 通知标题
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// 通知内容
    /// </summary>
    public string Content { get; set; }
    
    /// <summary>
    /// 是否已读
    /// </summary>
    public bool IsRead { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}