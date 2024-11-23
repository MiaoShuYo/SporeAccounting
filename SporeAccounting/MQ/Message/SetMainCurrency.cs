using SporeAccounting.Models;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.MQ.Message;

/// <summary>
/// 设置主货币
/// </summary>
public static class SetMainCurrency
{
    /// <summary>
    /// 开始监听
    /// </summary>
    public static void Start(IServiceProvider serviceProvider)
    {
        var subscriber = serviceProvider.GetRequiredService<RabbitMQSubscriber>();
        _ = subscriber.Subscribe("SetMainCurrency", "SetMainCurrency", async (userId) =>
        {
            var accountBookServer = serviceProvider.GetRequiredService<IConfigServer>();
            accountBookServer.Add(new Config()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Value = "CNY",
                ConfigTypeEnum = ConfigTypeEnum.Currency,
                CreateDateTime = DateTime.Now,
                CreateUserId = userId
            });
        });
    }
}