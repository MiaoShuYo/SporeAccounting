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
    [StringLength(50)]
    public string UserName { get; set; }
    
    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能小于6位")]
    public string Password { get; set; }
}