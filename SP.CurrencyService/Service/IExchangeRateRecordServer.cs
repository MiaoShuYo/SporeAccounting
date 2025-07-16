using SP.Common.Model;
using SP.CurrencyService.Models.Entity;
using SP.CurrencyService.Models.Request;
using SP.CurrencyService.Models.Response;

namespace SP.CurrencyService.Service;

// <summary>
// ICurrencyServer接口定义了货币服务的基本操作
// </summary>
public interface IExchangeRateRecordServer
{
    /// <summary>
    /// 添加汇率记录
    /// </summary>
    /// <param name="exchangeRateRecords">汇率记录</param>
    void Add(List<ExchangeRateRecord> exchangeRateRecords);

    /// <summary>
    /// 分页查询汇率
    /// </summary>
    /// <param name="exchangeRateRecordPage">分页请求</param>
    /// <returns>返回分页结果</returns>
    PageResponse<ExchangeRateRecordResponse> QueryByPage(
        ExchangeRateRecordPageRequestRequest exchangeRateRecordPage);

    /// <summary>
    /// 获取今日源币种和目标币种之间的汇率
    /// </summary>
    /// <param name="sourceCurrencyId">源币种</param>
    /// <param name="targetCurrencyId">目标币种</param>
    /// <returns>返回今日汇率记录</returns>
    ExchangeRateRecordResponse GetTodayExchangeRate(long sourceCurrencyId, long targetCurrencyId);
}