using SP.Common.Model;

namespace SP.FinanceService.Models.Request;

/// <summary>
/// 分页查询规则请求
/// </summary>
public class RecurringExpenseRulePgRequest:PageRequest
{
    /// <summary>
    /// 规则标题
    /// </summary>
    public string Title { get; set; }
}