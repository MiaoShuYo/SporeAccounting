using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Mvc;
using SporeAccounting.BaseModels;
using SporeAccounting.Models.ViewModels;
using SporeAccounting.Server;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Controllers;

/// <summary>
/// 币种控制器
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Consumer,Administrator")]
public class CurrencyController:BaseController
{
    /// <summary>
    /// 币种服务
    /// </summary>
    private readonly ICurrencyServer _currencyServer;
    /// <summary>
    /// 映射
    /// </summary>
    private readonly IMapper _mapper;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="currencyServer"></param>
    /// <param name="mapper"></param>
    public CurrencyController(ICurrencyServer currencyServer, IMapper mapper)
    {
        _currencyServer = currencyServer;
        _mapper = mapper;
    }
    
    /// <summary>
    /// 获取全部币种
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("QueryAll")]
    public ActionResult<ResponseData<List<CurrencyViewModel>>> QueryAll()
    {
        try
        {
            var currencies = _currencyServer.Query().ToList();
            if (currencies?.Count>0)
            {
                var currencyViewModels = _mapper.Map<List<CurrencyViewModel>>(currencies);
                return Ok(new ResponseData<List<CurrencyViewModel>>(HttpStatusCode.OK, data:currencyViewModels));
            }
            else
            {
                return Ok(new ResponseData<List<CurrencyViewModel>>(HttpStatusCode.NotFound, errorMessage:"数据不存在"));
            }
            
        }
        catch (Exception e)
        {
            return Ok(new ResponseData<List<CurrencyViewModel>>(HttpStatusCode.InternalServerError, errorMessage:e.Message));
        }
    }
}