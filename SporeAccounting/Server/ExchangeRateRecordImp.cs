using SporeAccounting.Models;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Server;

/// <summary>
/// 汇率记录服务实现
/// </summary>
public class ExchangeRateRecordImp : IExchangeRateRecordServer
{
    private readonly SporeAccountingDBContext _sporeAccountingDbContext;

    public ExchangeRateRecordImp(SporeAccountingDBContext sporeAccountingDbContext)
    {
        _sporeAccountingDbContext = sporeAccountingDbContext;
    }

    /// <summary>
    /// 批量新增汇率记录
    /// </summary>
    /// <param name="exchangeRateRecord"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Add(List<ExchangeRateRecord> exchangeRateRecord)
    {
        try
        {
            _sporeAccountingDbContext.ExchangeRateRecords.AddRange(exchangeRateRecord);
            _sporeAccountingDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }
}