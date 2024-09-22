using System.ComponentModel.DataAnnotations;

namespace SporeAccounting.Models.ViewModels;

public class SysUserEditViewModel
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    public string UserName { get; set; }
    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; }
    /// <summary>
    /// 手机号
    /// </summary>
    public string PhoneNumber { get; set; }
}