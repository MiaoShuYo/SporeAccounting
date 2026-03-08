using System.ComponentModel.DataAnnotations;
using SP.Common.Attributes;

namespace SP.IdentityService.Models.Request;

/// <summary>
/// 请求验证邮箱
/// </summary>
[ObjectRules(AnyOf = new[] { "Email", "PhoneNumber" })]
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
    [MaxLength(100, ErrorMessage = "邮箱长度不能超过100")]
    public string Email { get; set; }
    
    /// <summary>
    /// 手机号
    /// </summary>
    [MaxLength(20, ErrorMessage = "手机号长度不能超过20")]
    public string PhoneNumber { get; set; }
}