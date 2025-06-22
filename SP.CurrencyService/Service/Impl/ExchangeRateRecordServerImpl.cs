using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Model;
using SP.CurrencyService.DB;
using SP.CurrencyService.Models.Entity;
using SP.CurrencyService.Models.Request;
using SP.CurrencyService.Models.Response;

namespace SP.CurrencyService.Service.Impl;

/// <summary>
/// 汇率记录服务实现
/// </summary>
public class ExchangeRateRecordServerImpl : IExchangeRateRecordServer
{
    private readonly CurrencyServiceDbContext _dbContext;
    private readonly IMapper _mapper;

    public ExchangeRateRecordServerImpl(CurrencyServiceDbContext dbContext, IMapper mapper)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    /// <summary>
    /// 添加汇率记录
    /// </summary>
    /// <param name="exchangeRateRecords">汇率记录</param>
    public void Add(List<ExchangeRateRecord> exchangeRateRecords)
    {
        _dbContext.ExchangeRateRecords.AddRange(exchangeRateRecords);
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// 分页查询汇率
    /// </summary>
    /// <param name="exchangeRateRecordPage">分页查询请求</param>
    /// <returns></returns>
    public async Task<PageResponseModel<ExchangeRateRecordResponse>> QueryByPage(
        ExchangeRateRecordPageRequestRequest exchangeRateRecordPage)
    {
        if (exchangeRateRecordPage == null)
        {
            throw new ArgumentNullException(nameof(exchangeRateRecordPage));
        }

        var query = _dbContext.ExchangeRateRecords.AsQueryable();

        if (exchangeRateRecordPage.SourceCurrencyId > 0 && exchangeRateRecordPage.SourceCurrencyId > 0)
        {
            query = query.Where(x =>
                x.SourceCurrencyId == exchangeRateRecordPage.SourceCurrencyId &&
                x.TargetCurrencyId == exchangeRateRecordPage.TargetCurrencyId);
        }

        var pageIndex = exchangeRateRecordPage.PageIndex;
        var pageSize = exchangeRateRecordPage.PageSize;
        var skip = (pageIndex - 1) * pageSize;

        var totalCount = await query.CountAsync();

        var data = await query
            .OrderByDescending(x => x.Date)
            .Skip(skip)
            .Take(pageSize)
            .Select(x => new ExchangeRateRecordResponse
            {
                Id = x.Id,
                ConvertCurrency = x.ConvertCurrency,
                ExchangeRate = x.ExchangeRate,
                Date = x.Date
            })
            .ToListAsync();

        var page = new PageResponseModel<ExchangeRateRecordResponse>
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalCount = totalCount,
            Data = data,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };

        return page;
    }

    /// <summary>
    /// 获取今日源币种和目标币种之间的汇率
    /// </summary>
    /// <param name="sourceCurrencyId">源币种</param>
    /// <param name="targetCurrencyId">目标币种</param>
    /// <returns>返回今日汇率记录</returns>
    public async Task<ExchangeRateRecordResponse> GetTodayExchangeRate(long sourceCurrencyId,
        long targetCurrencyId)
    {
        var today = DateTime.Today;
        var todayExchangeRate = await _dbContext.ExchangeRateRecords
            .FirstOrDefaultAsync(x => x.Date.Date == today && x.SourceCurrencyId == sourceCurrencyId &&
                                      x.TargetCurrencyId == targetCurrencyId);
        if (todayExchangeRate == null)
        {
            throw new BusinessException("今日没有汇率记录");
        }

        ExchangeRateRecordResponse response = _mapper.Map<ExchangeRateRecordResponse>(todayExchangeRate);
        return response;
    }
}