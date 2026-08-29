using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.MLService.Models.Enumeration;
using SP.Common.Model;

namespace SP.MLService.Models.Entity;

/// <summary>
/// AI使用记录
/// </summary>
[Table(name: "AIUsageRecord")]
public class AIUsageRecord : BaseModel
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public long UserId { get; set; }

    /// <summary>
    /// 使用类型
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public AIUsageRecordTypeEnum UsageType { get; set; }
}