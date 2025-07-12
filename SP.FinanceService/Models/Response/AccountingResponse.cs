namespace SP.FinanceService.Models.Response;

/// <summary>
/// 记账响应模型
/// </summary>
public class AccountingResponse
{
    /// <summary>
    /// 记账记录Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 金额
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 收支分类Id
    /// </summary>
    public long TransactionCategoryId { get; set; }

    /// <summary>
    /// 收支分类名称
    /// </summary>
    public string TransactionCategoryName { get; set; }

    /// <summary>
    /// 记录日期
    /// </summary>
    public DateTime RecordDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 币种Id
    /// </summary>
    public long CurrencyId { get; set; }

    /// <summary>
    /// 币种名称
    /// </summary>
    public string CurrencyName { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}