using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Common.Model;

namespace SP.FinanceService.Models.Entity;

/// <summary>
/// 分摊结算流水表
/// </summary>
[Table("SharedExpenseSettlement")]
public class SharedExpenseSettlement : BaseModel
{
    /// <summary>
    /// 分摊账目Id
    /// </summary>
    [Column(TypeName = "bigint")]
    [Required]
    public long SharedExpenseId { get; set; }

    /// <summary>
    /// 参与者Id（还款人）
    /// </summary>
    [Column(TypeName = "bigint")]
    [Required]
    public long ParticipantId { get; set; }

    /// <summary>
    /// 收款人Id（发起人/垫付人）
    /// </summary>
    [Column(TypeName = "bigint")]
    [Required]
    public long ReceiverId { get; set; }

    /// <summary>
    /// 本次还款金额
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    [Required]
    public decimal PaymentAmount { get; set; }

    /// <summary>
    /// 还款方式（如：现金、支付宝、微信、银行转账）
    /// </summary>
    [Column(TypeName = "nvarchar(50)")]
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// 结算日期
    /// </summary>
    [Column(TypeName = "datetime")]
    [Required]
    public DateTime SettlementDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 关联的收入记账Id（收款人产生的收入记录）
    /// </summary>
    [Column(TypeName = "bigint")]
    [Required]
    public long AccountingId { get; set; }

    /// <summary>
    /// 结算凭证（如转账截图URL）
    /// </summary>
    [Column(TypeName = "nvarchar(500)")]
    public string? ProofUrl { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string? Remark { get; set; }
}
