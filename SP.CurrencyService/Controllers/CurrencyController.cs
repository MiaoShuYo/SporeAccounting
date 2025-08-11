using Microsoft.AspNetCore.Mvc;
using SP.CurrencyService.Models.Response;
using SP.CurrencyService.Service;

namespace SP.CurrencyService.Controllers;

/// <summary>
/// 币种控制器
/// </summary>
[Route("/api/currencies")]
[ApiController]
public class CurrencyController : ControllerBase
{
    private readonly ICurrencyServer _currencyServer;

    public CurrencyController(ICurrencyServer currencyServer)
    {
        _currencyServer = currencyServer;
    }

    /// <summary>
    /// 获取所有币种
    /// </summary>
    /// <returns>返回币种列表</returns>
    [HttpGet]
    public ActionResult<List<CurrencyResponse>> GetCurrencies()
    {
        List<CurrencyResponse> currencies = _currencyServer.Query().Result;
        return Ok(currencies);
    }
}