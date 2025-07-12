using SP.Common.Model;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service;

/// <summary>
/// 账本服务接口
/// </summary>
public interface IAccountBookServer
{
    /// <summary>
    /// 新增账本
    /// </summary>
    /// <param name="request">账本添加请求</param>
    /// <returns></returns>
    long Add(AccountBookAddRequest request);

    /// <summary>
    /// 删除账本
    /// </summary>
    /// <param name="id">账本id</param>
    void Delete(long id);

    /// <summary>
    /// 修改账本
    /// </summary>
    /// <param name="request"></param>
    void Edit(AccountBookEditeRequest request);

    /// <summary>
    /// 查询账本分页
    /// </summary>
    /// <param name="page">分页数据</param>
    /// <returns></returns>
    PageResponseModel<AccountBookResponse> QueryPage(AccountBookPageRequest page);
}