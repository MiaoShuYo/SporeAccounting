using SporeAccounting.Models;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Server;

/// <summary>
/// 币种服务实现
/// </summary>
public class CurrencyImp : ICurrencyServer
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

    /// <summary>
    /// 查询币种
    /// </summary>
    /// <param name="currencyId"></param>
    /// <returns></returns>
    public Currency? Query(string currencyId)
    {
        try
        {
            return _dbContext.Currencies.FirstOrDefault(c => c.Id == currencyId);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 查询币种
    /// </summary>
    /// <param name="currencyAbbreviation"></param>
    /// <returns></returns>
    public Currency? QueryByAbbreviation(string currencyAbbreviation)
    {
        try
        {
            return _dbContext.Currencies.FirstOrDefault(c => c.Abbreviation == currencyAbbreviation);
        }
        catch (Exception e)
        {
            throw;
        }
    }
}