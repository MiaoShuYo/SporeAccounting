using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Build.Framework;
using SporeAccounting.BaseModels;

namespace SporeAccounting.Models;

/// <summary>
/// 账簿
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
    /// 账簿余额
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    [Required] 
    public decimal Balance { get; set; } = 0;

    /// <summary>
    /// 备注
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    public string? Remarks { get; set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    [Column(TypeName = "nvarchar(36)")]
    [ForeignKey("User")]
    [Required]
    public string UserId { get; set; }

    /// <summary>
    /// 导航属性
    /// </summary>
    public SysUser User { get; set; }

    /// <summary>
    /// 导航属性
    /// </summary>
    public ICollection<IncomeExpenditureRecord> IncomeExpenditureRecords { get; set; } =
        new List<IncomeExpenditureRecord>();
}