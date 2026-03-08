using System.ComponentModel.DataAnnotations;
using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Request;

/// <summary>
/// 新增分摊账目请求
/// </summary>
public class SharedExpenseAddRequest
{
    /// <summary>
    /// 账本Id
    /// </summary>
    [Required(ErrorMessage = "账本Id不能为空")]
    public long AccountBookId { get; set; }

    /// <summary>
    /// 分摊标题
    /// </summary>
    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(100, ErrorMessage = "标题长度不能超过100个字")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 总金额
    /// </summary>
    [Required(ErrorMessage = "总金额不能为空")]
    [Range(0.01, double.MaxValue, ErrorMessage = "金额必须大于0")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 币种Id
    /// </summary>
    [Required(ErrorMessage = "币种Id不能为空")]
    public long CurrencyId { get; set; }

    /// <summary>
    /// 交易分类Id
    /// </summary>
    [Required(ErrorMessage = "交易分类Id不能为空")]
    public long TransactionCategoryId { get; set; }

    /// <summary>
    /// 支出日期
    /// </summary>
    [Required(ErrorMessage = "支出日期不能为空")]
    public DateTime ExpenseDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 分摊方式
    /// </summary>
    [Required(ErrorMessage = "分摊方式不能为空")]
    public SplitTypeEnum SplitType { get; set; }

    /// <summary>
    /// 参与者分摊明细
    /// </summary>
    [Required(ErrorMessage = "参与者不能为空")]
    [MinLength(1, ErrorMessage = "至少需要一个参与者")]
    public List<SharedExpenseParticipantAddRequest> Participants { get; set; } = new();

    /// <summary>
    /// 描述
    /// </summary>
    [MaxLength(500, ErrorMessage = "描述长度不能超过500个字")]
    public string? Description { get; set; }
}

/// <summary>
/// 分摊参与者明细请求
/// </summary>
public class SharedExpenseParticipantAddRequest
{
    /// <summary>
    /// 参与者Id
    /// </summary>
    [Required(ErrorMessage = "参与者Id不能为空")]
    public long ParticipantId { get; set; }

    /// <summary>
    /// 分摊金额
    /// </summary>
    [Required(ErrorMessage = "分摊金额不能为空")]
    [Range(0.01, double.MaxValue, ErrorMessage = "分摊金额必须大于0")]
    public decimal ShareAmount { get; set; }

    /// <summary>
    /// 分摊比例（按比例分摊时填写）
    /// </summary>
    [Range(0, 100, ErrorMessage = "分摊比例必须在0-100之间")]
    public decimal? ShareRatio { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(200, ErrorMessage = "备注长度不能超过200个字")]
    public string? Remark { get; set; }
}
