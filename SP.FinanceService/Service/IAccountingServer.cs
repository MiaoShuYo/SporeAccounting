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
}