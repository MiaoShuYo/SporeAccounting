using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SP.Common.Message.Model;
using SP.Common.Message.Mq.Model;
using SP.Common.Message.SmS.Services;

namespace SP.Common.Message.Mq.Consumer;

/// <summary>
/// 短信消费者服务
/// </summary>
public class SmSConsumerService : BackgroundService
{
    /// <summary>
    /// 日志
    /// </summary>
    private readonly ILogger<SmSConsumerService> _logger;

    /// <summary>
    /// RabbitMq 消息
    /// </summary>
    private readonly RabbitMqMessage _rabbitMqMessage;

    /// <summary>
    /// 短信服务
    /// </summary>
    private readonly ISmSService _smSService;

    /// <summary>
    /// 构造函数
    /// </summary>
    public SmSConsumerService(ILogger<SmSConsumerService> logger,
        RabbitMqMessage rabbitMqMessage,
        ISmSService smSService)
    {
        _logger = logger;
        _rabbitMqMessage = rabbitMqMessage;
        _smSService = smSService;
    }

    /// <summary>
    /// 执行异步任务
    /// </summary>
    /// <param name="stoppingToken"></param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        MqSubscriber subscriber = new MqSubscriber(MqExchange.MessageExchange,
            MqRoutingKey.SmsRoutingKey, MqQueue.SmSQueue);
        await _rabbitMqMessage.ReceiveAsync(subscriber, async message =>
        {
            MqMessage mqMessage = message as MqMessage;
            string body = mqMessage.Body;
            SmSMessage? smSMessage = JsonSerializer.Deserialize<SmSMessage>(body);
            if (smSMessage == null)
            {
                _logger.LogError("消息体解析失败");
                await Task.CompletedTask;
                return;
            }

            try
            {
                // 发送验证码
                if (mqMessage.Type == MessageType.SmSVerificationCode)
                {
                    await _smSService.SendVerificationCodeAsync(smSMessage.PhoneNumber, smSMessage.Purpose);
                }
                else if (mqMessage.Type == MessageType.SmSGeneral)
                {
                    await _smSService.SendMessageAsync(smSMessage.PhoneNumber, smSMessage.Message, smSMessage.Purpose);
                }
                else
                {
                    _logger.LogError("消息类型错误");
                    await Task.CompletedTask;
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "短信发送失败，消息将被确认以避免无限重试。MessageId={MessageId}, Phone={Phone}", mqMessage.Id, smSMessage.PhoneNumber);
                await Task.CompletedTask;
                return;
            }

            await Task.CompletedTask;
        }, stoppingToken);
    }
}