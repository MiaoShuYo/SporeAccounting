using SporeAccounting.Models;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Server;
/// <summary>
/// 币种服务实现
/// </summary>
public class CurrencyImp:ICurrencyService
{
    
    private readonly SporeAccountingDBContext _dbContext;
    public CurrencyImp(SporeAccountingDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    /// <summary>
    /// 查询全部币种
    /// </summary>
    /// <returns></returns>
    public IQueryable<Currency> Query()
    {
        try
        {
            return _dbContext.Currencies;
        }
        catch (Exception e)
        {
            throw;
        }
    }
}