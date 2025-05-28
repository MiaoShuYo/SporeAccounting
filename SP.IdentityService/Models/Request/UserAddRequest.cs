using System.ComponentModel.DataAnnotations;

namespace SP.IdentityService.Models.Request;

/// <summary>
/// 用户添加请求模型
/// </summary>
public class UserAddRequest
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "用户名长度必须在3-50个字符之间")]
    [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "用户名只能包含字母、数字、下划线和连字符")]
    public string UserName { get; set; }
    
    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码不能为空")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度必须在6-100个字符之间")]
    public string Password { get; set; }
}