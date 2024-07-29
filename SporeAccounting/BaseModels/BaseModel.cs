using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace SporeAccounting.BaseModels;

/// <summary>
/// 数据库映射类基类
/// </summary>
public class BaseModel
{
    /// <summary>
    /// 表数据唯一值
    /// </summary>
    [Key]
    [Column(TypeName = "nvarchar(36)")]
    [Required]
    public string Id { get; set; }
    /// <summary>
    /// 创建时间
    /// </summary>
    [Required]
    [Column(TypeName = "datetime")]
    public DateTime CreateDateTime { get; set; }
    /// <summary>
    /// 创建用户
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(36)")]
    public DateTime CreateUserId { get; set; }
    /// <summary>
    /// 修改时间
    /// </summary>
    [Required]
    [Column(TypeName = "datetime")]
    public DateTime UpdateDateTime { get; set; }
    /// <summary>
    /// 修改用户
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(36)")]
    public DateTime UpdateUserId { get; set; }
    /// <summary>
    /// 删除时间
    /// </summary>
    [Required]
    [Column(TypeName = "datetime")]
    public DateTime DeleteDateTime { get; set; }
    /// <summary>
    /// 删除用户
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(36)")]
    public DateTime DeleteUserId { get; set; }
    /// <summary>
    /// 是否删除（物理删除）
    /// </summary>
    [Required]
    [Column(TypeName = "bool")]
    public bool IsDeleted { get; set; }
}