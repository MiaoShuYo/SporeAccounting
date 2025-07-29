using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Common.Model;

namespace SP.FinanceService.Models.Entity;

/// <summary>
/// 记账实体
/// </summary>
public class Accounting: BaseModel
{
    /// <summary>
    /// 转换前金额
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    [Required]
    public decimal BeforAmount { get; set; }

    /// <summary>
    /// 转换后金额
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    [Required]
    public decimal AfterAmount { get; set; }

    /// <summary>
    /// 收支分类Id
    /// </summary>
    [Column(TypeName = "bigint")]
    [Required]
    public long TransactionCategoryId { get; set; }

    /// <summary>
    /// 记录日期
    /// </summary>
    [Column(TypeName = "datetime")]
    [Required]
    public DateTime RecordDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 账簿Id
    /// </summary>
    [Column(TypeName = "bigint")]
    [Required]
    public long AccountBookId { get; set; }

    /// <summary>
    /// 转换前币种Id
    /// </summary>
    [Column(TypeName = "bigint")]
    [Required]
    public long CurrencyId { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    public string? Remark { get; set; }
}