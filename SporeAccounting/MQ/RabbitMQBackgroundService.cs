using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SporeAccounting.Models;
using SporeAccounting.MQ.Message.Model;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.MQ
{
    /// <summary>
    /// RabbitMQBackgroundService
    /// </summary>
    public class RabbitMQBackgroundService : IHostedService
    {
        private readonly RabbitMQSubscriberService _subscriberService;
        private readonly ILogger<RabbitMQBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQBackgroundService(RabbitMQSubscriberService subscriberService,
            ILogger<RabbitMQBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _subscriberService = subscriberService;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async System.Threading.Tasks.Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting RabbitMQ subscription service...");

            // 配置多个队列订阅
            await _subscriberService.SubscribeAsync<MainCurrency>("SetMainCurrency", "SetMainCurrency",
                (mainCurrency) =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    var configService = scope.ServiceProvider.GetRequiredService<IConfigServer>();
                    configService.Add(new Config()
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = mainCurrency.UserId,
                        Value = mainCurrency.Currency,
                        ConfigTypeEnum = ConfigTypeEnum.Currency,
                        CreateDateTime = DateTime.Now,
                        CreateUserId = mainCurrency.UserId
                    });
                });
            await _subscriberService.SubscribeAsync<string>("UpdateConversionAmount", "UpdateConversionAmount",
                async (mainCurrencyId) =>
                {
                    //1.获取所有收支记录
                    var recordService = _serviceProvider.GetRequiredService<IIncomeExpenditureRecordServer>();
                    var records = recordService.Query();

                    //2.将所有记录的金额转换为新的主币种（记录中的币种转换为新的主币种）
                    var currencyServer = _serviceProvider.GetRequiredService<ICurrencyServer>();
                    var exchangeRateRecordServer = _serviceProvider.GetRequiredService<IExchangeRateRecordServer>();
                    Currency? mainCurrency = currencyServer.Query(mainCurrencyId);
                    if (mainCurrency == null)
                    {
                        return;
                    }

                    for (int i = 0; i < records.Count; i++)
                    {
                        var record = records[i];
                        var currency = record.Currency;
                        //获取记录币种和主币种的汇率
                        ExchangeRateRecord? exchangeRateRecord =
                            exchangeRateRecordServer.Query($"{mainCurrency.Abbreviation}_{currency.Abbreviation}");
                        record.AfterAmount = exchangeRateRecord.ExchangeRate * record.BeforAmount;
                    }

                    //3.更新所有记录
                    recordService.UpdateRecord(records);
                });
        }

        public System.Threading.Tasks.Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping RabbitMQ subscription service...");
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}