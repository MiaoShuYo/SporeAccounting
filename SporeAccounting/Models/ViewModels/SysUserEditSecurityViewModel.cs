using System.ComponentModel.DataAnnotations;

namespace SporeAccounting.Models.ViewModels;
/// <summary>
/// 用户安全信息修改视图模型
/// </summary>
public class SysUserEditSecurityViewModel
{
    /// <summary>
    /// 手机号
    /// </summary>
    [Required(ErrorMessage = "手机号不能为空")]
    [Phone(ErrorMessage = "手机号格式不正确")]
    public string PhoneNumber { get; set; }
    
    /// <summary>
    /// 邮箱
    /// </summary>
    [Required(ErrorMessage = "邮箱不能为空")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string Email { get; set; }
}