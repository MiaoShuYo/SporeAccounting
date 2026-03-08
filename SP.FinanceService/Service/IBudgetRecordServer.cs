using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service;

/// <summary>
/// 预算记录服务接口
/// </summary>
public interface IBudgetRecordServer
{
    /// <summary>
    /// 根据预算Id集合获取预算记录
    /// </summary>
    /// <returns>预算记录集合</returns>
    Dictionary<long, List<BudgetRecordResponse>> GetBudgetRecordsByBudgetIds();
}