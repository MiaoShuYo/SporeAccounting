using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Common.Model;
using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Entity;

/// <summary>
/// 财务健康评分记录
/// </summary>
[Table("FinancialHealthScore")]
public class FinancialHealthScore : BaseModel
{
    /// <summary>
    /// 账本 ID
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public long AccountBookId { get; set; }

    /// <summary>
    /// 总评分（0~100）
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal TotalScore { get; set; }

    /// <summary>
    /// 收支比率得分（权重 30%）
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal IncomeExpenseRatioScore { get; set; }

    /// <summary>
    /// 储蓄率得分（权重 30%）
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal SavingsRateScore { get; set; }

    /// <summary>
    /// 预算执行率得分（权重 25%，无预算数据时为 null）
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal? BudgetComplianceScore { get; set; }

    /// <summary>
    /// 收入稳定性得分（权重 15%）
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal IncomeStabilityScore { get; set; }

    /// <summary>
    /// 健康等级
    /// </summary>
    [Required]
    [Column(TypeName = "tinyint")]
    public HealthLevelEnum HealthLevel { get; set; }

    /// <summary>
    /// 统计周期开始日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime")]
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// 统计周期结束日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime")]
    public DateTime PeriodEnd { get; set; }
}
