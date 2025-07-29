using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Common.Model;
using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Entity;

/// <summary>
/// 预算实体
/// </summary>
public class Budget: BaseModel
{
    /// <summary>
    /// 收支类型
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public long TransactionCategoryId { get; set; }

    /// <summary>
    /// 预算金额
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    /// <summary>
    /// 预算周期
    /// </summary>
    [Column(TypeName = "tinyint")]
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
}