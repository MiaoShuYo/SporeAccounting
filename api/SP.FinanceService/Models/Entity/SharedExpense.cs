using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Common.Model;
using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Entity;

/// <summary>
/// 分摊账目表
/// </summary>
[Table(name: "SharedExpense")]
public class SharedExpense : BaseModel
{
    /// <summary>
    /// 账目总金额
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    [Required]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 分摊发起人（实际付款人）
    /// </summary>
    [Column(TypeName = "bigint")]
    [Required]
    public long PayerId { get; set; }

    /// <summary>
    /// 关联的支出记账记录Id（创建分摊时自动创建一笔支出记录）
    /// </summary>
    [Column(TypeName = "bigint")]
    [Required]
    public long AccountingId { get; set; }

    /// <summary>
    /// 账本Id
    /// </summary>
    [Column(TypeName = "bigint")]
    [Required]
    public long AccountBookId { get; set; }

    /// <summary>
    /// 分摊标题
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 币种Id
    /// </summary>
    [Column(TypeName = "bigint")]
    [Required]
    public long CurrencyId { get; set; }

    /// <summary>
    /// 交易分类Id
    /// </summary>
    [Column(TypeName = "bigint")]
    [Required]
    public long TransactionCategoryId { get; set; }

    /// <summary>
    /// 支出日期
    /// </summary>
    [Column(TypeName = "datetime")]
    [Required]
    public DateTime ExpenseDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 分摊方式
    /// </summary>
    [Column(TypeName = "tinyint")]
    [Required]
    public SplitTypeEnum SplitType { get; set; }

    /// <summary>
    /// 分摊状态
    /// </summary>
    [Column(TypeName = "tinyint")]
    [Required]
    public SharedExpenseStatusEnum Status { get; set; } = SharedExpenseStatusEnum.Unpaid;

    /// <summary>
    /// 描述
    /// </summary>
    [Column(TypeName = "nvarchar(500)")]
    public string? Description { get; set; }
}