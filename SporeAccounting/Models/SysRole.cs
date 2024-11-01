using SporeAccounting.BaseModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SporeAccounting.Models;

/// <summary>
/// 角色
/// </summary>
[Table(name: "SysRole")]
public class SysRole : BaseModel
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [Column(TypeName = "nvarchar(20)")]
    [Required]
    public string RoleName { get; set; }
    /// <summary>
    /// 是否允许删除
    /// </summary>
    [Column(TypeName = "tinyint(1)")]
    [Required]
    public bool CanDelete { get; set; } = true;
    /// <summary>
    /// 导航属性
    /// </summary>
    public ICollection<SysUser> Users { get; set; }
    /// <summary>
    /// 导航属性
    /// </summary>
    public ICollection<SysRoleUrl> RoleUrls { get; set; }
}