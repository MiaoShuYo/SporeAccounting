using System.Text.Json;
using Quartz;
using SporeAccounting.Models;
using SporeAccounting.Server.Interface;
using SporeAccounting.Task.Timer.Model;

namespace SporeAccounting.Task.Timer;

/// <summary>
/// 获取汇率定时器
/// </summary>
public class ExchangeRateTimer : IJob
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ICurrencyServer _currencyServer;

    public ExchangeRateTimer(IHttpClientFactory httpClientFactory,
        IConfiguration configuration, IServiceScopeFactory serviceScopeFactory,
        ICurrencyServer currencyServer)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _serviceScopeFactory = serviceScopeFactory;
        _currencyServer = currencyServer;
    }

    public System.Threading.Tasks.Task Execute(IJobExecutionContext context)
    {
        string exchangeRateUrl = _configuration["ExchangeRate"];

        //获取全部币种
        List<Currency> currencies = _currencyServer.Query().ToList();
        //获取对每种币种的汇率
        foreach (var currency in currencies)
        {
            _httpClientFactory.CreateClient().GetAsync($"{exchangeRateUrl}{currency.Abbreviation}")
                .ContinueWith(
                    response =>
                    {
                        using var scope = _serviceScopeFactory.CreateScope();
                        var exchangeRateRecordService =
                            scope.ServiceProvider.GetRequiredService<IExchangeRateRecordServer>();
                        List<ExchangeRateRecord> exchangeRateRecords = new();
                        if (response.Result.IsSuccessStatusCode)
                        {
                            var result = response.Result.Content.ReadAsStringAsync().Result;
                            var resultModel = JsonSerializer.Deserialize<ExchangeRateApiData>(result);
                            if (resultModel?.Result == "success")
                            {
                                foreach (var rate in resultModel.ConversionRates)
                                {
                                    //只获取人民币、日元、欧元、韩元、美元、港币、澳门元、英镑、新台币之间的汇率
                                    //其他币种的汇率直接跳过
                                    if (currencies.All(c => c.Abbreviation != rate.Key))
                                    {
                                        continue;
                                    }

                                    exchangeRateRecords.Add(new ExchangeRateRecord
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        ExchangeRate = rate.Value,
                                        //汇率记录的币种代码是基础币种代码和目标币种代码的组合
                                        ConvertCurrency = $"{resultModel.BaseCode}_{rate.Key}",
                                        Date = DateTime.Now,
                                        CreateDateTime = DateTime.Now,
                                        CreateUserId = "System",
                                        IsDeleted = false
                                    });
                                }
                                //存入数据库
                                exchangeRateRecordService.Add(exchangeRateRecords);
                            }
                        }
                    });
        }

        return System.Threading.Tasks.Task.CompletedTask;
    }
}