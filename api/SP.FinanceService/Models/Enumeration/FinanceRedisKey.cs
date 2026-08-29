namespace SP.FinanceService.Models.Enumeration;

public class FinanceRedisKey
{
    /// <summary>
    /// 定期支出规则key
    /// </summary>
    public const string RecurringExpenseKey = "FINANCE:RECURRINGEXPENSE";

    /// <summary>
    /// 用户定期支出规则key
    /// </summary>
    public const string RecurringExpenseUserKey = "FINANCE:RECURRINGEXPENSE:{0}";
}