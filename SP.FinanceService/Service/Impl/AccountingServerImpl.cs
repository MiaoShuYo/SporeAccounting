using SP.Common.Model;
using SP.FinanceService.DB;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service.Impl;

/// <summary>
/// 记账服务实现类
/// </summary>
public class AccountingServerImpl : IAccountingServer
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly FinanceServiceDbContext _dbContext;

    /// <summary>
    /// 记账服务构造函数
    /// </summary>
    /// <param name="dbContext"></param>
    public AccountingServerImpl(FinanceServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    /// <summary>
    /// 根据账本id查询是否存在记账数据
    /// </summary>
    /// <param name="accountBookId">账本id</param>
    /// <returns></returns>
    public bool AccountingExistByAccountBookId(long accountBookId)
    {
        // 查询账本下是否有记账数据
        return _dbContext.Accountings.Any(a => a.AccountBookId == accountBookId);
    }

    /// <summary>
    /// 新增记账
    /// </summary>
    /// <param name="request">记账添加请求</param>
    /// <returns></returns>
    public long Add(AccountingAddRequest request)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 删除记账
    /// </summary>
    /// <param name="id">记账ID</param>
    public void Delete(long id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 修改记账
    /// </summary>
    /// <param name="request">修改请求</param>
    public void Edit(AccountingEditRequest request)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 查询记账详情
    /// </summary>
    /// <param name="id">记账记录id</param>
    /// <returns>记账详情</returns>
    public AccountingResponse QueryById(long id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 查询记账分页
    /// </summary>
    /// <param name="page">分页请求</param>
    /// <returns>记账列表</returns>
    public PageResponse<AccountingResponse> QueryPage(AccountingPageRequest page)
    {
        throw new NotImplementedException();
    }
}