using SporeAccounting.Models;

namespace SporeAccounting.Server.Interface;

/// <summary>
/// 汇率记录服务接口
/// </summary>
public interface IExchangeRateRecordService
{
    /// <summary>
    /// 批量新增汇率记录
    /// </summary>
    /// <param name="exchangeRateRecord"></param>
    void Add(List<ExchangeRateRecord> exchangeRateRecord);
}