using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SP.Common;

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
    [Comment("Id")]
    public long Id { get; set; } = Snow.GetId();
    /// <summary>
    /// 创建时间
    /// </summary>
    [Required]
    [Column(TypeName = "datetime")]
    [Comment("创建时间")]
    public DateTime CreateDateTime { get; set; }= DateTime.Now;
    /// <summary>
    /// 创建用户
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(36)")]
    [Comment("创建用户")]
    public long CreateUserId { get; set; }
    /// <summary>
    /// 修改时间
    /// </summary>
    [Column(TypeName = "datetime")]
    [Comment("修改时间")]
    public DateTime? UpdateDateTime { get; set; }
    /// <summary>
    /// 修改用户
    /// </summary>
    [Column(TypeName = "nvarchar(36)")]
    [Comment("修改用户")]
    public long? UpdateUserId { get; set; }
    /// <summary>
    /// 是否删除
    /// </summary>
    [Required]
    [Column(TypeName = "tinyint(1)")]
    public bool IsDeleted { get; set; }=false;
}