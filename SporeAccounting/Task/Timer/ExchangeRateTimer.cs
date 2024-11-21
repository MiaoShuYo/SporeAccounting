using Quartz;

namespace SporeAccounting.Task.Timer;
/// <summary>
/// 获取汇率定时器
/// </summary>
public class ExchangeRateTimer:IJob
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration; 
    public ExchangeRateTimer(IHttpClientFactory httpClientFactory,IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }
    public System.Threading.Tasks.Task Execute(IJobExecutionContext context)
    {
        // string exchangeRateUrl = _configuration["ExchangeRate"];
        // //获取主币种
        // _httpClientFactory.CreateClient().GetAsync(exchangeRateUrl);
        return null;
    }
}