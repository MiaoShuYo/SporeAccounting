using System.ComponentModel.DataAnnotations;

namespace SP.FinanceService.Models.Request;

/// <summary>
/// 分摊结算添加请求
/// </summary>
public class SharedExpenseSettlementAddRequest
{
    /// <summary>
    /// 分摊账目Id
    /// </summary>
    [Required(ErrorMessage = "分摊账目Id不能为空")]
    public long SharedExpenseId { get; set; }

    /// <summary>
    /// 参与者Id（还款人）
    /// </summary>
    [Required(ErrorMessage = "参与者Id不能为空")]
    public long ParticipantId { get; set; }

    /// <summary>
    /// 收款人Id（发起人/垫付人）
    /// </summary>
    [Required(ErrorMessage = "收款人Id不能为空")]
    public long ReceiverId { get; set; }

    /// <summary>
    /// 本次还款金额
    /// </summary>
    [Required(ErrorMessage = "还款金额不能为空")]
    [Range(0.01, double.MaxValue, ErrorMessage = "还款金额必须大于0")]
    public decimal PaymentAmount { get; set; }

    /// <summary>
    /// 还款方式（如：现金、支付宝、微信、银行转账）
    /// </summary>
    [MaxLength(50, ErrorMessage = "还款方式长度不能超过50个字符")]
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// 结算日期
    /// </summary>
    [Required(ErrorMessage = "结算日期不能为空")]
    public DateTime SettlementDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 结算凭证（如转账截图URL）
    /// </summary>
    [MaxLength(500, ErrorMessage = "凭证长度不能超过500个字符")]
    public string? ProofUrl { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(200, ErrorMessage = "备注长度不能超过200个字符")]
    public string? Remark { get; set; }
}
