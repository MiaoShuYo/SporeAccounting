using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Response;

/// <summary>
/// 分摊账目响应
/// </summary>
public class SharedExpenseResponse
{
    /// <summary>
    /// 分摊账目Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 账目总金额
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 分摊发起人（实际付款人）
    /// </summary>
    public long PayerId { get; set; }

    /// <summary>
    /// 关联的支出记账记录Id
    /// </summary>
    public long AccountingId { get; set; }

    /// <summary>
    /// 账本Id
    /// </summary>
    public long AccountBookId { get; set; }

    /// <summary>
    /// 分摊标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 币种Id
    /// </summary>
    public long CurrencyId { get; set; }

    /// <summary>
    /// 交易分类Id
    /// </summary>
    public long TransactionCategoryId { get; set; }

    /// <summary>
    /// 支出日期
    /// </summary>
    public DateTime ExpenseDate { get; set; }

    /// <summary>
    /// 分摊方式
    /// </summary>
    public SplitTypeEnum SplitType { get; set; }

    /// <summary>
    /// 分摊状态
    /// </summary>
    public SharedExpenseStatusEnum Status { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 参与者明细
    /// </summary>
    public List<SharedExpenseParticipantResponse> Participants { get; set; } = new();
}

/// <summary>
/// 分摊参与者响应
/// </summary>
public class SharedExpenseParticipantResponse
{
    /// <summary>
    /// 分摊参与者Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 分摊账目Id
    /// </summary>
    public long SharedExpenseId { get; set; }

    /// <summary>
    /// 分摊参与者Id
    /// </summary>
    public long ParticipantId { get; set; }

    /// <summary>
    /// 应分摊金额
    /// </summary>
    public decimal ShareAmount { get; set; }

    /// <summary>
    /// 分摊比例
    /// </summary>
    public decimal? ShareRatio { get; set; }

    /// <summary>
    /// 是否已结算
    /// </summary>
    public bool IsPaid { get; set; }

    /// <summary>
    /// 结算时间
    /// </summary>
    public DateTime? SettlementDate { get; set; }

    /// <summary>
    /// 关联的收入记账Id
    /// </summary>
    public long? AccountingId { get; set; }

    /// <summary>
    /// 结算凭证
    /// </summary>
    public string? ProofUrl { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}
