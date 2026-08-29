using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Common.Model;

namespace SP.FinanceService.Models.Entity;

/// <summary>
/// 账本
/// </summary>
[Table(name: "AccountBook")]
public class AccountBook : BaseModel
{
    /// <summary>
    /// 账簿名称
    /// </summary>
    [Column(TypeName = "nvarchar(20)")]
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    public string? Remarks { get; set; }
}