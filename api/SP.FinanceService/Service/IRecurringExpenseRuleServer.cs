using SP.Common.Model;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service;

/// <summary>
/// 定期支出规则服务接口
/// </summary>
public interface IRecurringExpenseRuleServer
{
    /// <summary>
    /// 添加定期支出规则
    /// </summary>
    /// <param name="recurringExpenseRuleAdd">定期支出参数</param>
    /// <returns></returns>
    Task<long> AddRecurringExpenseRule(RecurringExpenseRuleAddRequest recurringExpenseRuleAdd);

    /// <summary>
    /// 修改定期支出规则
    /// </summary>
    /// <param name="recurringExpenseRuleEdit">定期支出参数</param>
    /// <returns></returns>
    Task<long> EditRecurringExpenseRule(RecurringExpenseRuleEditRequest recurringExpenseRuleEdit);

    /// <summary>
    /// 删除定期支出规则
    /// </summary>
    /// <param name="ids">定期支出规则id集合</param>
    System.Threading.Tasks.Task DeleteRecurringExpenseRule(List<long> ids);
    /// <summary>
    /// 获取定期支出规则
    /// </summary>
    /// <param name="id">定期支出规则id</param>
    /// <returns></returns>
    RecurringExpenseRuleResponse GetRecurringExpenseRule(long id);

    /// <summary>
    /// 分页查询定期支出规则
    /// </summary>
    /// <param name="page">分页参数</param>
    /// <returns></returns>
    PageResponse<RecurringExpenseRuleResponse> GetRecurringExpenseRulePage(RecurringExpenseRulePgRequest page);

    /// <summary>
    /// 获取全部定期支出规则
    /// </summary>
    /// <returns></returns>
    Task<List<RecurringExpenseRuleResponse>> GetAllRecurringExpenseRules();
}