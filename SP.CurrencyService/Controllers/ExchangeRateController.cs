using Microsoft.AspNetCore.Mvc;
using SP.Common.Model;
using SP.CurrencyService.Models.Request;
using SP.CurrencyService.Models.Response;
using SP.CurrencyService.Service;

namespace SP.CurrencyService.Controllers;

/// <summary>
/// 汇率控制器
/// </summary>
[Route("/api/exchange-rates")]
[ApiController]
public class ExchangeRateController : ControllerBase
{
    /// <summary>
    /// 汇率记录服务
    /// </summary>
    private readonly IExchangeRateRecordServer _exchangeRateRecordServer;

    /// <summary>
    /// 汇率控制器构造函数
    /// </summary>
    /// <param name="exchangeRateRecordServer">汇率记录服务</param>
    public ExchangeRateController(IExchangeRateRecordServer exchangeRateRecordServer)
    {
        _exchangeRateRecordServer = exchangeRateRecordServer;
    }

    /// <summary>
    /// 分页获取汇率记录列表
    /// </summary>
    /// <param name="sourceCurrencyId">源币种ID</param>
    /// <param name="targetCurrencyId">目标币种ID</param>
    /// <param name="page">页码</param>
    /// <param name="size">每页数量</param>
    /// <returns>返回分页结果</returns>
    [HttpGet]
    public ActionResult<PageResponse<ExchangeRateRecordResponse>> GetExchangeRates(
        [FromQuery] long sourceCurrencyId,
        [FromQuery] long targetCurrencyId,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        var request = new ExchangeRateRecordPageRequestRequest
        {
            SourceCurrencyId = sourceCurrencyId,
            TargetCurrencyId = targetCurrencyId,
            PageIndex = page,
            PageSize = size
        };
        PageResponse<ExchangeRateRecordResponse> response = _exchangeRateRecordServer.QueryByPage(request);
        return Ok(response);
    }

    /// <summary>
    /// 获取指定币种对的今日汇率
    /// </summary>
    /// <param name="sourceCurrencyId">源币种ID</param>
    /// <param name="targetCurrencyId">目标币种ID</param>
    /// <returns>返回今日汇率记录</returns>
    [HttpGet("{sourceCurrencyId}/{targetCurrencyId}/today")]
    public ActionResult<ExchangeRateRecordResponse> GetTodayExchangeRate(
        [FromRoute] long sourceCurrencyId, [FromRoute] long targetCurrencyId)
    {
        ExchangeRateRecordResponse response = _exchangeRateRecordServer.GetTodayExchangeRate(
            sourceCurrencyId, targetCurrencyId);
        return Ok(response);
    }
}