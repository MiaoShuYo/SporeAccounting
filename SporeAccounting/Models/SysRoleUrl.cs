using SporeAccounting.BaseModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SporeAccounting.Models;
/// <summary>
/// 角色可访问的URL
/// </summary>
[Table(name: "SysRoleUrl")]
public class SysRoleUrl : BaseModel
{
    /// <summary>
    /// 角色Id
    /// </summary>
    [Column(TypeName = "nvarchar(36)")]
    [Required]
    [ForeignKey("FK_SysRoleUrl_SysRole")]
    public string RoleId { get; set; }
    /// <summary>
    /// 接口路径
    /// </summary>
    [Column(TypeName = "nvarchar(36)")]
    [Required]
    [ForeignKey("FK_SysRoleUrl_SysUrl")]
    public string UrlId { get; set; }
    /// <summary>
    /// 是否允许删除
    /// </summary>
    [Column(TypeName = "tinyint(1)")]
    [Required]
    public bool CanDelete { get; set; }= false;

    /// <summary>
    /// 导航属性
    /// </summary>
    public SysRole Role { get; set; }
    /// <summary>
    /// 导航属性
    /// </summary>
    public SysUrl Url { get; set; }
}