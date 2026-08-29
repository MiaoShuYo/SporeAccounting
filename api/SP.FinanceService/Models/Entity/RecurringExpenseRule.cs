using SP.Common.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.FinanceService.Models.Entity;

/// <summary>
/// 规定开销规则
/// </summary>
public class RecurringExpenseRule: BaseModel
{
    /// <summary>
    /// 账本id
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public long AccountBookId { get; set; }
    /// <summary>
    /// 标题
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(20)")]
    public string Title { get; set; }
    /// <summary>
    /// 金额
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    /// <summary>
    /// 分类id
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public long CategoryId { get; set; }
    /// <summary>
    /// 开始日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime")]
    public DateTime StartDate { get; set; }
    /// <summary>
    /// 结束日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime")]
    public DateTime EndDate { get; set; }
    /// <summary>
    /// 频率（单位：天）
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public int FrequencyInDays { get; set; }
    /// <summary>
    /// 币种id
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public long CurrencyId { get; set; }
}