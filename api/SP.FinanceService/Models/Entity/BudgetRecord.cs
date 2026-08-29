using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Common.Model;

namespace SP.FinanceService.Models.Entity;

/// <summary>
/// 预算记录实体
/// </summary>
public class BudgetRecord : BaseModel
{
    /// <summary>
    /// 预算ID
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public long BudgetId { get; set; }

    /// <summary>
    /// 记录日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime")]
    public DateTime RecordDate { get; set; }

    /// <summary>
    /// 使用金额（正数为消耗，负数为回补）
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UsedAmount { get; set; }
}