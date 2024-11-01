using System.ComponentModel.DataAnnotations;

namespace SporeAccounting.Models.ViewModels;

/// <summary>
/// 角色视图模型
/// </summary>
public class SysRoleViewModel
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [Required(ErrorMessage = "角色名称不能为空")]
    [MaxLength(20)]
    public string RoleName { get; set; }
}