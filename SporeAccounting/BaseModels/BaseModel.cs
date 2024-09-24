using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    public string Id { get; set; }= Guid.NewGuid().ToString();
    /// <summary>
    /// 创建时间
    /// </summary>
    [Required]
    [Column(TypeName = "datetime")]
    public DateTime CreateDateTime { get; set; }= DateTime.Now;
    /// <summary>
    /// 创建用户
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(36)")]
    public string CreateUserId { get; set; }
    /// <summary>
    /// 修改时间
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? UpdateDateTime { get; set; }
    /// <summary>
    /// 修改用户
    /// </summary>
    [Column(TypeName = "nvarchar(36)")]
    public string? UpdateUserId { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? DeleteDateTime { get; set; }
    /// <summary>
    /// 删除用户
    /// </summary>
    [Column(TypeName = "nvarchar(36)")]
    public string? DeleteUserId { get; set; }
    /// <summary>
    /// 是否删除（物理删除）
    /// </summary>
    [Required]
    [Column(TypeName = "tinyint(1)")]
    public bool IsDeleted { get; set; }=false;
}