using SporeAccounting.BaseModels;
using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;
using SporeAccounting.MQ.Message.Model;
using SporeAccounting.Server.Interface;
using System.Net;

namespace SporeAccounting.MQ.Message;

/// <summary>
/// 更改主币种后重新计算所有记录的金额
/// </summary>
public class UpdateConversionAmount
{
    /// <summary>
    /// 开始监听
    /// </summary>
    public static void Start(IServiceProvider serviceProvider)
    {
        var subscriber = serviceProvider.GetRequiredService<RabbitMQSubscriber>();
        _ = subscriber.Subscribe<string>("UpdateConversionAmount", "UpdateConversionAmount",
            async (mainCurrencyId) =>
            {
                //1.获取所有收支记录
                var recordService = serviceProvider.GetRequiredService<IIncomeExpenditureRecordServer>();
                var records = recordService.Query();

                //2.将所有记录的金额转换为新的主币种（记录中的币种转换为新的主币种）
                var currencyServer = serviceProvider.GetRequiredService<ICurrencyServer>();
                var exchangeRateRecordServer = serviceProvider.GetRequiredService<IExchangeRateRecordServer>();
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
                    record.AfterAmount = exchangeRateRecord.ExchangeRate*record.BeforAmount;
                }
                //3.更新所有记录
                recordService.UpdateRecord(records);
            });
    }
}