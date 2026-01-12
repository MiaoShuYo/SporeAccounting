using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Response;

public class RecurringExpenseRuleResponse
{
    
    /// <summary>
    /// id
    /// </summary>
    public long Id { get; set; }
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
    public DateTime StartDate { get; set; }
    /// <summary>
    /// 结束日期
    /// </summary>
    public DateTime EndDate { get; set; }
    /// <summary>
    /// 频率（0：天，1：周，2：月，3：季度，4：年）
    /// </summary>
    public FrequencyEnum Frequency { get; set; }
    /// <summary>
    /// 币种id
    /// </summary>
    public long CurrencyId { get; set; }
    /// <summary>
    /// 创建人
    /// </summary>
    public long CreateUserId { get; set; }
}