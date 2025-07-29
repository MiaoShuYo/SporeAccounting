using AutoMapper;
using SP.CurrencyService.DB;
using SP.CurrencyService.Models.Entity;
using SP.CurrencyService.Models.Response;

namespace SP.CurrencyService.Service.Impl;

/// <summary>
/// 货币服务实现
/// </summary>
public class CurrencyServerImpl : ICurrencyServer
{
    private readonly CurrencyServiceDbContext _dbContext;
    private readonly IMapper _mapper;

    public CurrencyServerImpl(CurrencyServiceDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    /// <summary>
    /// 查询所有货币
    /// </summary>
    /// <returns>返回货币列表</returns>
    public List<CurrencyResponse> Query()
    {
        List<Currency> crCurrencies = _dbContext.Currencies.ToList();
        List<CurrencyResponse> currencyResponses = _mapper.Map<List<CurrencyResponse>>(crCurrencies);
        return currencyResponses;
    }
}