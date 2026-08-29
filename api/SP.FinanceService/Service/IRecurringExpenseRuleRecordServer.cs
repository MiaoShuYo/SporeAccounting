using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service;

/// <summary>
/// 定期支出规则记录服务接口
/// </summary>
public interface IRecurringExpenseRuleRecordServer
{
    /// <summary>
    /// 查询定期支出执行记录
    /// </summary>
    /// <param name="id">定期支出规则id</param>
    /// <returns>执行记录</returns>
    RecurringExpenseRuleExecutionRecord GetRecordById(long id);

    /// <summary>
    /// 记录执行记录
    /// </summary>
    /// <param name="recurringExpense"></param>
    /// <returns></returns>
    void Add(RecurringExpenseRuleExecutionRecord recurringExpense);
}