using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SporeAccounting.BaseModels;
using SporeAccounting.BaseModels.ViewModel.Response;
using SporeAccounting.Models.ViewModels;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Controllers;

/// <summary>
/// 汇率控制器
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles="Consumer,Administrator")]
public class ExchangeRateController : BaseController
{
    private readonly IExchangeRateRecordServer _exchangeRateRecordServer;
    private readonly ICurrencyServer _currencyServer;
    private readonly IMapper _mapper;

    public ExchangeRateController(IExchangeRateRecordServer exchangeRateRecordServer, ICurrencyServer currencyServer,
        IMapper mapper)
    {
        _exchangeRateRecordServer = exchangeRateRecordServer;
        _mapper = mapper;
        _currencyServer = currencyServer;
    }

    /// <summary>
    /// 分页获取汇率
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("Query/{page}/{pageSize}")]
    public ActionResult<ResponseData<PageResponseViewModel<ExchangeRateRecordViewModel>>> Query(int page, int pageSize)
    {
        try
        {
            var (rowCount, exchangeRateRecords) = _exchangeRateRecordServer.Query((page - 1) * pageSize, pageSize);
            var exchangeRateRecordViewModels = _mapper.Map<List<ExchangeRateRecordViewModel>>(exchangeRateRecords);
            // 获取币种
            foreach (var exchangeRateRecordViewModel in exchangeRateRecordViewModels)
            {
                string[] convertCurrency = exchangeRateRecordViewModel.ConvertCurrency.Split('_');
                var currency1 = _currencyServer.QueryByAbbreviation(convertCurrency[0]);
                var currency2 = _currencyServer.QueryByAbbreviation(convertCurrency[1]);
                exchangeRateRecordViewModel.Currency1 = currency1?.Name;
                exchangeRateRecordViewModel.Currency2 = currency2?.Name;
            }

            PageResponseViewModel<ExchangeRateRecordViewModel> pageResponseViewModel =
                new PageResponseViewModel<ExchangeRateRecordViewModel>
                {
                    RowCount = rowCount,
                    Data = exchangeRateRecordViewModels,
                    PageCount = (int)Math.Ceiling(rowCount / (double)pageSize),
                };
            return Ok(new ResponseData<PageResponseViewModel<ExchangeRateRecordViewModel>>(HttpStatusCode.OK,
                data: pageResponseViewModel));
        }
        catch (Exception e)
        {
            return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError,
                errorMessage: e.Message));
        }
    }
}