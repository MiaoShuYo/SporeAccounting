using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SporeAccounting.BaseModels;
using SporeAccounting.BaseModels.ViewModel.Response;
using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Controllers
{
    /// <summary>
    /// 收支记录控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Consumer,Administrator")]
    public class IncomeExpenditureRecordController : BaseController
    {
        /// <summary>
        /// 收支记录服务
        /// </summary>
        private readonly IIncomeExpenditureRecordServer _incomeExpenditureRecordServer;

        /// <summary>
        /// 配置服务
        /// </summary>
        private readonly IConfigServer _configServer;

        /// <summary>
        /// 汇率记录服务
        /// </summary>
        private readonly IExchangeRateRecordServer _exchangeRateRecordServer;

        /// <summary>
        /// 币种服务
        /// </summary>
        private readonly ICurrencyServer _currencyServer;

        /// <summary>
        /// 映射器
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="incomeExpenditureRecordServer"></param>
        /// <param name="configServer"></param>
        /// <param name="exchangeRateRecordServer"></param>
        /// <param name="currencyServer"></param>
        /// <param name="mapper"></param>
        public IncomeExpenditureRecordController(IIncomeExpenditureRecordServer incomeExpenditureRecordServer,
            IConfigServer configServer,
            IExchangeRateRecordServer exchangeRateRecordServer,
            ICurrencyServer currencyServer,
            IMapper mapper)
        {
            _incomeExpenditureRecordServer = incomeExpenditureRecordServer;
            _configServer = configServer;
            _exchangeRateRecordServer = exchangeRateRecordServer;
            _currencyServer = currencyServer;
            _mapper = mapper;
        }

        /// <summary>
        /// 添加收支记录
        /// </summary>
        /// <param name="incomeExpenditureRecordAddViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Add")]
        public ActionResult<ResponseData<bool>> Add(
            [FromBody] IncomeExpenditureRecordAddViewModel incomeExpenditureRecordAddViewModel)
        {
            try
            {
                string userId = GetUserId();
                //获取用户设置的主币种
                Config? config = _configServer.Query(userId, ConfigTypeEnum.Currency);
                if (config == null)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, "未设置主币种"));
                }

                // 如果选择的币种不是设置的主币种，则将金额转换为主币种的金额
                if (config.Value != incomeExpenditureRecordAddViewModel.CurrencyId)
                {
                    //查询主币种
                    Currency? mainCurrency = _currencyServer.Query(config.Value);
                    if (mainCurrency == null)
                    {
                        return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, "币种不存在"));
                    }

                    // 查询传入的币种
                    Currency? recordCurrency = _currencyServer.Query(incomeExpenditureRecordAddViewModel.CurrencyId);
                    if (recordCurrency == null)
                    {
                        return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, "币种不存在"));
                    }

                    //获取记录币种和主币种的汇率
                    ExchangeRateRecord? exchangeRateRecord =
                        _exchangeRateRecordServer.Query($"{mainCurrency.Abbreviation}_{recordCurrency.Abbreviation}");

                    if (exchangeRateRecord == null)
                    {
                        return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, "汇率不存在"));
                    }

                    incomeExpenditureRecordAddViewModel.AfterAmount = exchangeRateRecord.ExchangeRate*incomeExpenditureRecordAddViewModel.BeforAmount;
                }

                IncomeExpenditureRecord incomeExpenditureRecord =
                    _mapper.Map<IncomeExpenditureRecord>(incomeExpenditureRecordAddViewModel);
                incomeExpenditureRecord.UserId = userId;
                incomeExpenditureRecord.CreateDateTime = DateTime.Now;
                incomeExpenditureRecord.CreateUserId = userId;
                _incomeExpenditureRecordServer.Add(incomeExpenditureRecord);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务端异常"));
            }
        }

        /// <summary>
        /// 删除收支记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete/{id}")]
        public ActionResult<ResponseData<bool>> Delete([FromRoute] string id)
        {
            try
            {
                bool isExist = _incomeExpenditureRecordServer.IsExist(id);
                if (!isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, "记录不存在"));
                }

                _incomeExpenditureRecordServer.Delete(id);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务端异常"));
            }
        }

        /// <summary>
        /// 修改收支记录
        /// </summary>
        /// <param name="incomeExpenditureRecordUpdateViewModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Update")]
        public ActionResult<ResponseData<bool>> Update(
            [FromBody] IncomeExpenditureRecordEditViewModel incomeExpenditureRecordUpdateViewModel)
        {
            try
            {
                string userId = GetUserId();
                bool isExist =
                    _incomeExpenditureRecordServer.IsExist(incomeExpenditureRecordUpdateViewModel
                        .IncomeExpenditureRecordId);
                if (!isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, "记录不存在"));
                }

                //获取用户设置的主币种
                Config? config = _configServer.Query(userId, ConfigTypeEnum.Currency);
                if (config == null)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, "未设置主币种"));
                }

                // 如果选择的币种不是设置的主币种，则将金额转换为主币种的金额
                if (config.Value != incomeExpenditureRecordUpdateViewModel.CurrencyId)
                {
                    //查询主币种
                    Currency? mainCurrency = _currencyServer.Query(config.Value);
                    if (mainCurrency == null)
                    {
                        return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, "币种不存在"));
                    }

                    // 查询传入的币种
                    Currency? recordCurrency = _currencyServer.Query(incomeExpenditureRecordUpdateViewModel.CurrencyId);
                    if (recordCurrency == null)
                    {
                        return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, "币种不存在"));
                    }

                    //获取记录币种和主币种的汇率
                    ExchangeRateRecord? exchangeRateRecord =
                        _exchangeRateRecordServer.Query($"{mainCurrency.Abbreviation}_{recordCurrency.Abbreviation}");

                    if (exchangeRateRecord == null)
                    {
                        return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, "汇率不存在"));
                    }

                    incomeExpenditureRecordUpdateViewModel.AfterAmount = exchangeRateRecord.ExchangeRate *
                                                                         incomeExpenditureRecordUpdateViewModel
                                                                             .BeforAmount;
                }


                IncomeExpenditureRecord incomeExpenditureRecord =
                    _mapper.Map<IncomeExpenditureRecord>(incomeExpenditureRecordUpdateViewModel);
                incomeExpenditureRecord.UpdateDateTime = DateTime.Now;
                incomeExpenditureRecord.UpdateUserId = userId;
                _incomeExpenditureRecordServer.Update(incomeExpenditureRecord);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务端异常"));
            }
        }

        /// <summary>
        /// 查询收支记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Query/{id}")]
        public ActionResult<ResponseData<IncomeExpenditureRecordInfoViewModel>> Query([FromRoute] string id)
        {
            try
            {
                IncomeExpenditureRecord? incomeExpenditureRecord = _incomeExpenditureRecordServer.Query(id);
                if (incomeExpenditureRecord == null)
                {
                    return Ok(new ResponseData<IncomeExpenditureRecord>(HttpStatusCode.NotFound, "记录不存在"));
                }

                IncomeExpenditureRecordInfoViewModel incomeExpenditureRecordInfo =
                    _mapper.Map<IncomeExpenditureRecordInfoViewModel>(incomeExpenditureRecord);
                return Ok(new ResponseData<IncomeExpenditureRecordInfoViewModel>(HttpStatusCode.OK,
                    data: incomeExpenditureRecordInfo));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<IncomeExpenditureRecordInfoViewModel>(HttpStatusCode.InternalServerError,
                    "服务端异常"));
            }
        }

        /// <summary>
        /// 分页查询收支记录
        /// </summary>
        /// <param name="incomeExpenditureRecordPageViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Query")]
        public ActionResult<ResponseData<PageResponseViewModel<IncomeExpenditureRecordInfoViewModel>>> Query(
            [FromBody] IncomeExpenditureRecordPageViewModel incomeExpenditureRecordPageViewModel)
        {
            try
            {
                string userId = GetUserId();
                var (rowCount, pageCount, incomeExpenditureClassifications) = _incomeExpenditureRecordServer.Query(
                    incomeExpenditureRecordPageViewModel.PageNumber,
                    incomeExpenditureRecordPageViewModel.PageSize, userId,
                    incomeExpenditureRecordPageViewModel.StartDate, incomeExpenditureRecordPageViewModel.EndDate);

                List<IncomeExpenditureRecordInfoViewModel> incomeExpenditureRecordInfoViewModels =
                    _mapper.Map<List<IncomeExpenditureRecordInfoViewModel>>(incomeExpenditureClassifications);
                PageResponseViewModel<IncomeExpenditureRecordInfoViewModel> pageDataInfo =
                    new PageResponseViewModel<IncomeExpenditureRecordInfoViewModel>
                    {
                        RowCount = rowCount,
                        Data = incomeExpenditureRecordInfoViewModels,
                        PageCount = pageCount
                    };

                return Ok(new ResponseData<PageResponseViewModel<IncomeExpenditureRecordInfoViewModel>>(
                    HttpStatusCode.OK,
                    data: pageDataInfo));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<PageResponseViewModel<IncomeExpenditureRecordInfoViewModel>>(
                    HttpStatusCode.InternalServerError,
                    "服务端异常"));
            }
        }
    }
}