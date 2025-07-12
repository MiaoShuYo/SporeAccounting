using SP.FinanceService.DB;

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
}