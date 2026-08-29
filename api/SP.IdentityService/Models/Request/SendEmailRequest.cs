using System.ComponentModel.DataAnnotations;

namespace SP.IdentityService.Models.Request;

/// <summary>
/// 请求邮箱
/// </summary>
public class SendEmailRequest
{
    /// <summary>
    /// 邮箱地址
    /// </summary>
    [Required(ErrorMessage = "邮箱不能为空")]
    [StringLength(100, ErrorMessage = "邮箱长度不能超过100个字符")]
    public string Email { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    [Required(ErrorMessage = "消息类型不能为空")]
    public string MessageType { get; set; }
}