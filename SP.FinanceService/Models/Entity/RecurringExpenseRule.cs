using SP.Common.Model;

namespace SP.FinanceService.Models.Entity;

/// <summary>
/// 规定开销规则
/// </summary>
public class RecurringExpenseRule: BaseModel
{
    /// <summary>
    /// 账本id
    /// </summary>
    public long AccountBookId { get; set; }
    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// 金额
    /// </summary>
    public decimal Amount { get; set; }
    /// <summary>
    /// 分类id
    /// </summary>
    public long CategoryId { get; set; }
    /// <summary>
    /// 开始日期
    /// </summary>
    public DateOnly StartDate { get; set; }
    /// <summary>
    /// 结束日期
    /// </summary>
    public DateOnly? EndDate { get; set; }
    /// <summary>
    /// 频率（单位：天）
    /// </summary>
    public int FrequencyInDays { get; set; }
    /// <summary>
    /// 币种id
    /// </summary>
    public long CurrencyId { get; set; }
}