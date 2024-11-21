using Quartz;
using SporeAccounting.Models;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Task.Timer;

/// <summary>
/// 获取汇率定时器
/// </summary>
public class ExchangeRateTimer : IJob
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ICurrencyService _currencyService;

    public ExchangeRateTimer(IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ICurrencyService currencyService)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _currencyService = currencyService;
    }

    public System.Threading.Tasks.Task Execute(IJobExecutionContext context)
    {
        string exchangeRateUrl = _configuration["ExchangeRate"];
        //获取全部币种
        List<Currency> currencies = _currencyService.Query().ToList();
        List<ExchangeRateRecord> exchangeRateRecords = new();
        //获取对每种币种的汇率
        foreach (var currency in currencies)
        {
            _httpClientFactory.CreateClient().GetAsync($"{exchangeRateUrl}{currency.Abbreviation}").ContinueWith(response =>
            {
                if (response.Result.IsSuccessStatusCode)
                {
                    var result = response.Result.Content.ReadAsStringAsync().Result;
                    
                }
            });
        }
       
        return null;
    }
}