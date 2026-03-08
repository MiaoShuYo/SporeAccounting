using System.ComponentModel.DataAnnotations;

namespace SP.NotificationService.Models.Request;

/// <summary>
/// 编辑站内通知请求模型
/// </summary>
public class EditInSiteNotificationRequest
{
    /// <summary>
    /// 通知ID
    /// </summary>
    [Required(ErrorMessage = "通知ID不能为空")]
    public long Id { get; set; }
    
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