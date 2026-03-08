using System.ComponentModel.DataAnnotations;
using SP.Common.Attributes;
using SP.IdentityService.Models.Enumeration;

namespace SP.IdentityService.Models.Request;

[ObjectRules(AnyOf = new[] { "Email", "PhoneNumber" })]
public class PasswordResetRequest
{
    /// <summary>
    /// 邮箱
    /// </summary>
    [StringLength(100, ErrorMessage = "邮箱长度不能超过100个字符")]
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [MaxLength(20, ErrorMessage = "手机号长度不能超过20")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// 验证码
    /// </summary>
    [Required(ErrorMessage = "验证码不能为空")]
    public string ResetCode { get; set; }

    /// <summary>
    /// 新密码
    /// </summary>
    [Required(ErrorMessage = "新密码不能为空")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度必须在6-100个字符之间")]
    public string NewPassword { get; set; }

    /// <summary>
    /// 找回方式
    /// </summary>
    [Required(ErrorMessage = "找回方式不能为空")]
    public ResetEnum ResetBy { get; set; }
}