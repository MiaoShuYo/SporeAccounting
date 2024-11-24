using SporeAccounting.Models;

namespace SporeAccounting.Server.Interface;

/// <summary>
/// 汇率记录服务接口
/// </summary>
public interface IExchangeRateRecordServer
{
    /// <summary>
    /// 批量新增汇率记录
    /// </summary>
    /// <param name="exchangeRateRecord"></param>
    void Add(List<ExchangeRateRecord> exchangeRateRecord);

    /// <summary>
    /// 查询汇率记录
    /// </summary>
    /// <param name="convertCurrency"></param>
    /// <returns></returns>
    ExchangeRateRecord? Query(string convertCurrency);
}