using System.ComponentModel.DataAnnotations;

namespace SP.FinanceService.Models.Request;

/// <summary>
/// 记账记录编辑请求模型
/// </summary>
public class AccountingEditRequest
{
    /// <summary>
    /// 记账记录Id
    /// </summary>
    [Required(ErrorMessage = "记账记录Id不能为空")]
    public long Id { get; set; }

    /// <summary>
    /// 金额
    /// </summary>
    [Required(ErrorMessage = "金额不能为空")]
    public decimal Amount { get; set; }

    /// <summary>
    /// 收支分类Id
    /// </summary>
    [Required(ErrorMessage = "收支分类Id不能为空")]
    public long TransactionCategoryId { get; set; }

    /// <summary>
    /// 账簿Id
    /// </summary>
    [Required(ErrorMessage = "账本Id不能为空")]
    public long AccountBookId { get; set; }

    /// <summary>
    /// 记录日期
    /// </summary>
    [Required(ErrorMessage = "记录日期不能为空")]
    public DateTime RecordDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 币种Id
    /// </summary>
    [Required(ErrorMessage = "币种Id不能为空")]
    public long CurrencyId { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(100, ErrorMessage = "备注最大长度为100")]
    public string? Remark { get; set; }
}