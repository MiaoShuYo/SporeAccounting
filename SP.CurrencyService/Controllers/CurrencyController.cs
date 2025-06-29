using Microsoft.AspNetCore.Mvc;
using SP.CurrencyService.Models.Response;
using SP.CurrencyService.Service;

namespace SP.CurrencyService.Controllers;

/// <summary>
/// 币种控制器
/// </summary>
[Route("/api/currency")]
[ApiController]
public class CurrencyController : ControllerBase
{
    private readonly ICurrencyServer _currencyServer;

    public CurrencyController(ICurrencyServer currencyServer)
    {
        _currencyServer = currencyServer;
    }
    
    /// <summary>
    /// 查询所有币种
    /// </summary>
    /// <returns>返回币种列表</returns>
    [HttpGet("query")]
    public ActionResult<List<CurrencyResponse>> Query()
    {
        List<CurrencyResponse> currencies = _currencyServer.Query();
        return Ok(currencies);
    }
}