namespace SP.FinanceService.Models.Response;

/// <summary>
/// 账本响应模型
/// </summary>
public class AccountBookResponse
{
    /// <summary>
    /// 账本ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 账本名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    /// 账本收入金额
    /// </summary>
    public decimal IncomeAmount { get; set; }

    /// <summary>
    /// 账本支出金额
    /// </summary>
    public decimal ExpenditureAmount { get; set; }
}