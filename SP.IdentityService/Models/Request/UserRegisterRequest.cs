using System.ComponentModel.DataAnnotations;
using SP.Common.Attributes;
using SP.IdentityService.Models.Enumeration;

namespace SP.IdentityService.Models.Request;

/// <summary>
/// 用户注册请求模型
/// </summary>
[ObjectRules(AnyOf = new[] { "UserName", "Email", "PhoneNumber" },
    RequireIfPresent = new[] { "UserName=>Password", "Email=>Code", "PhoneNumber=>Code" })]
public class UserRegisterRequest
{
    /// <summary>
    /// 用户名
    /// </summary>
    [StringLength(50, MinimumLength = 3, ErrorMessage = "用户名长度必须在3-50个字符之间")]
    [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "用户名只能包含字母、数字、下划线和连字符")]
    public string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度必须在6-100个字符之间")]
    public string Password { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [StringLength(100, ErrorMessage = "邮箱长度不能超过100个字符")]
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [Phone(ErrorMessage = "手机号格式不正确")]
    [StringLength(20, ErrorMessage = "手机号长度不能超过20个字符")]
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// 验证码
    /// </summary>
    public string Code { get; set; }
    
    /// <summary>
    /// 注册类型
    /// </summary>
    [Required(ErrorMessage = "注册类型不能为空")]
    public RegisterTypeEnum RegisterType { get; set; }
}