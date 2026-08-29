using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SP.Common.Message.Model;
using SP.Common.Message.Mq.Model;

namespace SP.Common.Message.Mq.Consumer;

/// <summary>
/// 死信消息消费者
/// </summary>
public class DeadLetterConsumerService : BackgroundService
{
    private readonly RabbitMqMessage _rabbitMqMessage;
    private readonly ILogger<DeadLetterConsumerService> _logger;

    public DeadLetterConsumerService(RabbitMqMessage rabbitMqMessage, ILogger<DeadLetterConsumerService> logger)
    {
        _rabbitMqMessage = rabbitMqMessage;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        MqSubscriber subscriber = new MqSubscriber(
            MqExchange.DeadLetterExchange,
            MqRoutingKey.DeadLetterRoutingKey,
            MqQueue.DeadLetterQueue);

        await _rabbitMqMessage.ReceiveAsync(subscriber, async message =>
        {
            _logger.LogError("收到死信消息：MessageId={MessageId}, Type={MessageType}", message.Id, message.Type);
            await Task.CompletedTask;
        }, stoppingToken);
    }
}
