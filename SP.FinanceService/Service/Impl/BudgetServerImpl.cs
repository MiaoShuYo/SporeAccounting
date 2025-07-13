using SP.Common.Model;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service.Impl;

/// <summary>
/// 预算服务器实现类
/// </summary>
public class BudgetServerImpl : IBudgetServer
{
    /// <summary>
    /// 新增预算
    /// </summary>
    /// <param name="budget">预算</param>
    /// <returns>预算id</returns>
    public long Add(BudgetAddRequest budget)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 删除预算
    /// </summary>
    /// <param name="id">预算id</param>
    public void Delete(long id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 修改预算
    /// </summary>
    /// <param name="budget">修改预算</param>
    public void Edit(BudgetEditRequest budget)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 查询预算分页列表
    /// </summary>
    /// <param name="request">分页查询</param>
    /// <returns>预算列表</returns>
    public PageResponse<BudgetResponse> QueryPage(BudgetPageRequest request)
    {
        throw new NotImplementedException();
    }
}