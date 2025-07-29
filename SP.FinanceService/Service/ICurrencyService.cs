using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service;

/// <summary>
/// 货币服务接口
/// </summary>
public interface ICurrencyService
{
    /// <summary>
    /// 获取两个币种之间的今日汇率
    /// </summary>
    /// <param name="sourceCurrencyId">源币种ID</param>
    /// <param name="targetCurrencyId">目标币种ID</param>
    /// <returns>返回今日汇率记录</returns>
    Task<ExchangeRateRecordResponse> GetTodayExchangeRateByCode(long sourceCurrencyId, long targetCurrencyId);
} 