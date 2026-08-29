using System.ComponentModel.DataAnnotations;

namespace SP.NotificationService.Models.Request;

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
    [Required(ErrorMessage = "通知标题不能为空")]
    [MaxLength(255, ErrorMessage = "通知标题不能超过255个字符")]
    public string Title { get; set; }
    
    /// <summary>
    /// 通知内容
    /// </summary>
    [Required(ErrorMessage = "通知内容不能为空")]
    public string Content { get; set; }
}