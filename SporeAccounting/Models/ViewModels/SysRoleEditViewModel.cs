using System.ComponentModel.DataAnnotations;

namespace SporeAccounting.Models.ViewModels;
/// <summary>
/// 修改角色视图类
/// </summary>
public class SysRoleEditViewModel
{
    /// <summary>
    /// 角色id
    /// </summary>
    [Required(ErrorMessage = "角色Id不能为空")]
    public string RoleId { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    [Required(ErrorMessage = "角色名称不能为空")]
    public string RoleName { get; set; }
}