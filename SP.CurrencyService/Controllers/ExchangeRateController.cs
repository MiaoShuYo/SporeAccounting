using Microsoft.AspNetCore.Mvc;
using SP.Common.Model;
using SP.CurrencyService.Models.Request;
using SP.CurrencyService.Models.Response;
using SP.CurrencyService.Service;

namespace SP.CurrencyService.Controllers;

/// <summary>
/// 汇率控制器
/// </summary>
public class ExchangeRateController : ControllerBase
{
    private readonly IExchangeRateRecordServer _exchangeRateRecordServer;
    private readonly ICurrencyServer _currencyServer;

    public ExchangeRateController(IExchangeRateRecordServer exchangeRateRecordServer, ICurrencyServer currencyServer)
    {
        _exchangeRateRecordServer = exchangeRateRecordServer;
        _currencyServer = currencyServer;
    }

    /// <summary>
    /// 查询汇率记录分页
    /// </summary>
    /// <param name="request">分页请求</param>
    /// <returns>返回分页结果</returns>
    [HttpPost("queryByPage")]
    public async Task<ActionResult<PageResponseModel<ExchangeRateRecordResponse>>> QueryByPage(
        [FromBody] ExchangeRateRecordPageRequestRequest request)
    {
        PageResponseModel<ExchangeRateRecordResponse> response = await _exchangeRateRecordServer.QueryByPage(request);
        return Ok(response);
    }

    /// <summary>
    /// 获取今日源币种和目标币种之间的汇率
    /// </summary>
    /// <param name="sourceCurrencyId">源币种ID</param>
    /// <param name="targetCurrencyId">目标币种ID</param>
    /// <returns>返回今日汇率记录</returns>
    [HttpGet("getTodayExchangeRate/{sourceCurrencyId}/{targetCurrencyId}")]
    public async Task<ActionResult<List<ExchangeRateRecordResponse>>> GetTodayExchangeRate(
        long sourceCurrencyId, long targetCurrencyId)
    {
        ExchangeRateRecordResponse response = await _exchangeRateRecordServer.GetTodayExchangeRate(
            sourceCurrencyId, targetCurrencyId);
        return Ok(response);
    }
}