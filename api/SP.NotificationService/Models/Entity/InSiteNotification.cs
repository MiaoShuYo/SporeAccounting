using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Common.Model;

namespace SP.NotificationService.Models.Entity;

/// <summary>
/// 站内通知实体
/// </summary>
public class InSiteNotification: BaseModel
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public long UserId { get; set; }
    
    /// <summary>
    /// 通知标题
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(255)")]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// 通知内容
    /// </summary>
    [Required]
    [Column(TypeName = "text")]
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// 是否已读
    /// </summary>
    [Required]
    [Column(TypeName = "tinyint(1)")]
    public bool IsRead { get; set; } = false;
}