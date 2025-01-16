using SporeAccounting.Models;
using SporeAccounting.MQ.Message.Model;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.MQ
{
    /// <summary>
    /// RabbitMQ后台服务
    /// </summary>
    public class RabbitMQBackgroundService : IHostedService
    {
        private readonly RabbitMQSubscriberService _subscriberService;
        private readonly ILogger<RabbitMQBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="subscriberService"></param>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
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

            //设置主币种
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
                        ConfigType = ConfigTypeEnum.Currency,
                        CreateDateTime = DateTime.Now,
                        CreateUserId = mainCurrency.UserId
                    });
                });
            //根据新的主币种更新所有收支记录的金额
            await _subscriberService.SubscribeAsync<MainCurrency>("UpdateConversionAmount", "UpdateConversionAmount",
                async (mainCurrency) =>
                {
                    //1.获取所有收支记录
                    using var scope = _serviceProvider.CreateScope();
                    var recordService = scope.ServiceProvider.GetRequiredService<IIncomeExpenditureRecordServer>();
                    var records = recordService.QueryByUserId(mainCurrency.UserId);

                    //2.将所有记录的金额转换为新的主币种（记录中的币种转换为新的主币种）
                    var currencyServer = scope.ServiceProvider.GetRequiredService<ICurrencyServer>();
                    var exchangeRateRecordServer = scope.ServiceProvider.GetRequiredService<IExchangeRateRecordServer>();
                    Currency? query = currencyServer.Query(mainCurrency.Currency);
                    if (query == null)
                    {
                        return;
                    }
                    Currency? oldCurrency = currencyServer.Query(mainCurrency.OldCurrency);
                    if (oldCurrency == null)
                    {
                        return;
                    }
                    //获取预算币种和主币种的汇率
                    ExchangeRateRecord? exchangeRateRecord =
                        exchangeRateRecordServer.Query($"{oldCurrency.Abbreviation}_{query.Abbreviation}");
                    if(exchangeRateRecord == null)
                    {
                        return;
                    }
                    for (int i = 0; i < records.Count; i++)
                    {
                        var record = records[i];
                        if (exchangeRateRecord != null)
                            record.AfterAmount = exchangeRateRecord.ExchangeRate * record.BeforAmount;
                    }

                    //3.更新所有记录
                    recordService.UpdateRecord(records);
                });

            //根据新的主币种更新预算金额
            await _subscriberService.SubscribeAsync<MainCurrency>("UpdateBudgetAmount", "UpdateBudgetAmount",
                async (mainCurrency) =>
                {
                    //1.获取所有预算
                    using var scope = _serviceProvider.CreateScope();
                    var budgetServer = scope.ServiceProvider.GetRequiredService<IBudgetServer>();
                    var budgets = budgetServer.Query(mainCurrency.UserId);

                    //2.将所有预算的金额转换为新的主币种（预算中的币种转换为新的主币种）
                    var currencyServer = scope.ServiceProvider.GetRequiredService<ICurrencyServer>();
                    var exchangeRateRecordServer = scope.ServiceProvider.GetRequiredService<IExchangeRateRecordServer>();
                    Currency? query = currencyServer.Query(mainCurrency.Currency);
                    if (query == null)
                    {
                        return;
                    }
                    Currency? oldCurrency = currencyServer.Query(mainCurrency.OldCurrency);
                    if (oldCurrency == null)
                    {
                        return;
                    }
                    //获取预算币种和主币种的汇率
                    ExchangeRateRecord? exchangeRateRecord =
                        exchangeRateRecordServer.Query($"{oldCurrency.Abbreviation}_{query.Abbreviation}");
                    if(exchangeRateRecord == null)
                    {
                        return;
                    }
                    for (int i = 0; i < budgets.Count; i++)
                    {
                        var budget = budgets[i];
                        budget.Amount = exchangeRateRecord.ExchangeRate * budget.Amount;
                        budget.Remaining = exchangeRateRecord.ExchangeRate * budget.Remaining;
                    }

                    //3.更新所有预算
                    budgetServer.Update(budgets);
                });
        }

        public System.Threading.Tasks.Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping RabbitMQ subscription service...");
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}