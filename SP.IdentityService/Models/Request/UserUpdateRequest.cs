using System.ComponentModel.DataAnnotations;

namespace SP.IdentityService.Models.Request;

/// <summary>
/// 用户修改请求
/// </summary>
public class UserUpdateRequest
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "用户名长度必须在3-50个字符之间")]
    [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "用户名只能包含字母、数字、下划线和连字符")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 电子邮件
    /// </summary>
    [MaxLength(100, ErrorMessage = "电子邮件不能超过100字")]
    public string Email { get; set; } = string.Empty;
}