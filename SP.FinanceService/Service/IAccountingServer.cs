using SP.Common.Model;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service;

/// <summary>
/// 记账服务接口
/// </summary>
public interface IAccountingServer
{
    /// <summary>
    /// 根据账本id查询是否存在记账数据
    /// </summary>
    /// <param name="accountBookId">账本id</param>
    /// <returns></returns>
    bool AccountingExistByAccountBookId(long accountBookId);

    /// <summary>
    /// 新增记账
    /// </summary>
    /// <param name="accountBookId">账本ID</param>
    /// <param name="request">记账添加请求</param>
    /// <returns></returns>
    long Add(long accountBookId,AccountingAddRequest request);

    /// <summary>
    /// 删除记账
    /// </summary>
    /// <param name="accountBookId">账本ID</param>
    /// <param name="id">记账ID</param>
    void Delete(long accountBookId,long id);

    /// <summary>
    /// 修改记账
    /// </summary>
    /// <param name="accountBookId">账本ID</param>
    /// <param name="request">修改请求</param>
    void Edit(long accountBookId,AccountingEditRequest request);
    
    /// <summary>
    /// 查询记账详情
    /// </summary>
    /// <param name="accountBookId">账本ID</param>
    /// <param name="id">记账记录id</param>
    /// <returns>记账详情</returns>
    AccountingResponse QueryById(long accountBookId,long id);
    
    /// <summary>
    /// 查询记账分页
    /// </summary>
    /// <param name="accountBookId">账本ID</param>
    /// <param name="page">分页请求</param>
    /// <returns>记账列表</returns>
    PageResponse<AccountingResponse> QueryPage(long accountBookId,AccountingPageRequest page);
}