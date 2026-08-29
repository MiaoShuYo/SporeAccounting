using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Common.Model;

namespace SP.FinanceService.Models.Entity;

/// <summary>
/// 分摊参与者
/// </summary>
[Table("SharedExpenseParticipant")]
public class SharedExpenseParticipant : BaseModel
{
    /// <summary>
    /// 分摊账目Id
    /// </summary>
    [Column(TypeName = "bigint")]
    [Required]
    public long SharedExpenseId { get; set; }

    /// <summary>
    /// 分摊参与者Id
    /// </summary>
    [Column(TypeName = "bigint")]
    [Required]
    public long ParticipantId { get; set; }

    /// <summary>
    /// 应分摊金额（计算后的实际金额）
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    [Required]
    public decimal ShareAmount { get; set; }

    /// <summary>
    /// 分摊比例（用于按比例分摊，百分比存储，如 25.5 表示 25.5%）
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal? ShareRatio { get; set; }

    /// <summary>
    /// 是否已结算
    /// </summary>
    [Column(TypeName = "tinyint(1)")]
    [Required]
    public bool IsPaid { get; set; } = false;

    /// <summary>
    /// 结算时间
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? SettlementDate { get; set; }

    /// <summary>
    /// 关联的收入记账Id（参与者还款时，发起人产生的收入记录）
    /// </summary>
    [Column(TypeName = "bigint")]
    public long? AccountingId { get; set; }

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