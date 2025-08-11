using AutoMapper;
using SP.Common.Redis;
using SP.CurrencyService.DB;
using SP.CurrencyService.Models.Entity;
using SP.CurrencyService.Models.Enumeration;
using SP.CurrencyService.Models.Response;

namespace SP.CurrencyService.Service.Impl;

/// <summary>
/// 货币服务实现
/// </summary>
public class CurrencyServerImpl : ICurrencyServer
{
    private readonly CurrencyServiceDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IRedisService _redisService;
    private readonly string _spRedisKey = "";

    public CurrencyServerImpl(CurrencyServiceDbContext dbContext, IMapper mapper, IRedisService redisService)
    {
        _dbContext = dbContext;
        _redisService = redisService;
        _mapper = mapper;
        _spRedisKey = CurrencyRedisKey.Currency;
    }

    /// <summary>
    /// 查询所有货币
    /// </summary>
    /// <returns>返回货币列表</returns>
    public async Task<List<CurrencyResponse>> Query()
    {
        // 尝试从Redis缓存中获取币种列表
        List<CurrencyResponse>? cachedCurrencies = await _redisService.GetAsync<List<CurrencyResponse>>(_spRedisKey);
        if (cachedCurrencies != null)
        {
            return cachedCurrencies;
        }
        List<Currency> crCurrencies = _dbContext.Currencies.ToList();
        List<CurrencyResponse> currencyResponses = _mapper.Map<List<CurrencyResponse>>(crCurrencies);
        return currencyResponses;
    }
}