using SporeAccounting.BaseModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SporeAccounting.Models;
/// <summary>
/// 角色可访问的URL
/// </summary>
[Table(name: "SysRoleUrl")]
public class SysRoleUrl: BaseModel
{
    /// <summary>
    /// 角色Id
    /// </summary>
    [Column(TypeName = "nvarchar(36)")]
    [Required]
    public string RoleId { get; set; }
    /// <summary>
    /// 接口路径
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    [Required]
    public string Url { get; set; }

    /// <summary>
    /// 导航属性
    /// </summary>
    public ICollection<SysRole> Roles { get; set; }
}