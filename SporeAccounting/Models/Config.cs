using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SporeAccounting.BaseModels;

namespace SporeAccounting.Models;

/// <summary>
/// 用户配置表
/// </summary>
[Table(name: "Config")]
public class Config : BaseModel
{
    /// <summary>
    /// 配置类型
    /// </summary>
    [Column(TypeName = "int")]
    [Required]
    public ConfigTypeEnum ConfigType { get; set; }

    /// <summary>
    /// 用户id
    /// </summary>
    [ForeignKey("FK_Config_SysUser")]
    [Required]
    [Column(TypeName = "nvarchar(36)")]
    public string UserId { get; set; }
    /// <summary>
    /// 配置的值
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    [Required]
    public string Value { get; set; }
    
    /// <summary>
    /// 导航属性
    /// </summary>
    public SysUser User { get; set; }
}