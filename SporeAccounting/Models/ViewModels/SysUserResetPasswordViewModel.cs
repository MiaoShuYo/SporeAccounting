using System.ComponentModel.DataAnnotations;

namespace SporeAccounting.Models.ViewModels;
/// <summary>
/// 用户重置密码视图模型
/// </summary>
public class SysUserResetPasswordViewModel
{
    /// <summary>
    /// 用户Id
    /// </summary>
    [Required(ErrorMessage = "用户Id不能为空")]
    public string OldPassword { get; set; }

    /// <summary>
    /// 新密码
    /// </summary>
    [Required(ErrorMessage = "新密码不能为空")]
    public string NewPassword { get; set; }
}