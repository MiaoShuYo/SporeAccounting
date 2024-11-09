using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SporeAccounting.BaseModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SporeAccounting.Models;

/// <summary>
/// 接口URL表
/// </summary>
[Table("SysUrl")]
public class SysUrl : BaseModel
{
    /// <summary>
    /// 接口URL
    /// </summary>
    [Column(TypeName="nvarchar(200)")]
    [Required]
    public string Url { get; set; }
    /// <summary>
    /// URL描述
    /// </summary>
    [Column(TypeName="nvarchar(200)")]
    public string Description { get; set; }
    /// <summary>
    /// 是否可以删除
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    [Required]
    public bool CanDelete { get; set; } = true;
    /// <summary>
    /// 导航属性
    /// </summary>
    public ICollection<SysRoleUrl> RoleUrls { get; set; }
}