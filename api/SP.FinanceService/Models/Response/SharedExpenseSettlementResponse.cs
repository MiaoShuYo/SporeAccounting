namespace SP.FinanceService.Models.Response;

/// <summary>
/// 分摊结算响应
/// </summary>
public class SharedExpenseSettlementResponse
{
    /// <summary>
    /// 结算记录Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 分摊账目Id
    /// </summary>
    public long SharedExpenseId { get; set; }

    /// <summary>
    /// 参与者Id（还款人）
    /// </summary>
    public long ParticipantId { get; set; }

    /// <summary>
    /// 收款人Id（发起人/垫付人）
    /// </summary>
    public long ReceiverId { get; set; }

    /// <summary>
    /// 本次还款金额
    /// </summary>
    public decimal PaymentAmount { get; set; }

    /// <summary>
    /// 还款方式
    /// </summary>
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// 结算日期
    /// </summary>
    public DateTime SettlementDate { get; set; }

    /// <summary>
    /// 关联的收入记账Id
    /// </summary>
    public long AccountingId { get; set; }

    /// <summary>
    /// 结算凭证
    /// </summary>
    public string? ProofUrl { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}
