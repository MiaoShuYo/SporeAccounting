using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service;

/// <summary>
/// 分摊结算服务接口
/// </summary>
public interface ISharedExpenseSettlementServer
{
    /// <summary>
    /// 新增结算记录
    /// </summary>
    /// <param name="request">结算请求</param>
    /// <returns>结算记录Id</returns>
    long Add(SharedExpenseSettlementAddRequest request);

    /// <summary>
    /// 获取结算详情
    /// </summary>
    /// <param name="id">结算记录Id</param>
    /// <returns>结算详情</returns>
    SharedExpenseSettlementResponse QueryById(long id);

    /// <summary>
    /// 根据分摊账目Id查询结算记录列表
    /// </summary>
    /// <param name="sharedExpenseId">分摊账目Id</param>
    /// <returns>结算记录列表</returns>
    List<SharedExpenseSettlementResponse> QueryBySharedExpenseId(long sharedExpenseId);
}
