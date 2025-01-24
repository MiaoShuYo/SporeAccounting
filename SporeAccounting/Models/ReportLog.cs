using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SporeAccounting.BaseModels;

namespace SporeAccounting.Models;

/// <summary>
/// 报表日志
/// </summary>
[Table("ReportLog")]
public class ReportLog:BaseModel
{
    /// <summary>
    /// 用户Id
    /// </summary>
    [Column(TypeName = "nvarchar(36)")]
    [Required]
    public string UserId { get; set; }

    /// <summary>
    /// 报表Id
    /// </summary>
    [Column(TypeName = "nvarchar(36)")]
    [Required]
    public string ReportId { get; set; }
    
    /// <summary>
    /// 导航属性
    /// </summary>
    public SysUser User { get; set; }
}