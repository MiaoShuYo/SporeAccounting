using System.ComponentModel.DataAnnotations;

namespace SP.IdentityService.Models.Request;

/// <summary>
/// 请求验证邮箱
/// </summary>
public class VerifyCodeRequest
{
    /// <summary>
    /// 验证码
    /// </summary>
    [Required(ErrorMessage = "验证码不能为空")]
    public string Code { get; set; }

    /// <summary>
    /// 邮箱地址
    /// </summary>
    [Required(ErrorMessage = "邮箱不能为空")]
    [MaxLength(100, ErrorMessage = "邮箱长度不能超过100")]
    public string Email { get; set; }
}