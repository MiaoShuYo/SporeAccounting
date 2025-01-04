using System.ComponentModel.DataAnnotations;
using SporeAccounting.BaseModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace SporeAccounting.Models;

/// <summary>
/// 预算表
/// </summary>
[Table(name: "Budget")]
public class Budget : BaseModel
{
    /// <summary>
    /// 收支类型
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(36)")]
    public string IncomeExpenditureClassificationId { get; set; }

    /// <summary>
    /// 预算金额
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    /// <summary>
    /// 预算周期
    /// </summary>
    [Column(TypeName = "int")]
    [Required]
    public PeriodEnum Period { get; set; }

    /// <summary>
    /// 剩余预算
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Remaining { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(200)]
    public string? Remark { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    [Required]
    [Column(TypeName = "datetime")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    [Required]
    [Column(TypeName = "datetime")]
    public DateTime EndTime { get; set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(36)")]
    [ForeignKey("FK_Budget_SysUser")]
    public string UserId { get; set; }

    /// <summary>
    /// 导航属性
    /// </summary>
    public SysUser SysUser { get; set; }

    /// <summary>
    /// 导航属性
    /// </summary>
    public IncomeExpenditureClassification Classification { get; set; }
        = new IncomeExpenditureClassification();
}