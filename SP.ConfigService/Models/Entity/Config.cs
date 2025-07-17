using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Common.Model;
using SP.Config.Models.Enumeration;

namespace SP.Config.Models.Entity;

/// <summary>
/// 用户配置实体
/// </summary>
[Table("Config")]
public class Config:BaseModel
{
    /// <summary>
    /// 配置类型
    /// </summary>
    [Column(TypeName = "tinyint")]
    [Required]
    public ConfigTypeEnum ConfigType { get; set; }

    /// <summary>
    /// 用户id
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public long UserId { get; set; }
    /// <summary>
    /// 配置的值
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    [Required]
    public string Value { get; set; }
}