using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Message.Mq.Model;

namespace SP.Common.Message.Mq;

/// <summary>
/// RabbitMQ消息类
/// </summary>
public class RabbitMqMessage
{
    private readonly ILogger<RabbitMqMessage> _logger;
    private readonly RabbitMqConfig _rabbitMqConfig;

    /// <summary>
    /// RabbitMQ消息类构造函数
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="rabbitMqConfig"></param>
    public RabbitMqMessage(ILogger<RabbitMqMessage> logger, RabbitMqConfig rabbitMqConfig)
    {
        _rabbitMqConfig = rabbitMqConfig;
        _logger = logger;
    }

    /// <summary>
    /// 发送消息到RabbitMQ
    /// </summary>
    /// <param name="publisher"></param>
    /// <returns></returns>
    public async Task SendAsync(MqPublisher publisher)
    {
        // 检查参数
        if (publisher == null)
            throw new ArgumentNullException(nameof(publisher));
        if (string.IsNullOrWhiteSpace(publisher.Queue))
            throw new ArgumentException("队列名称不能为空", nameof(publisher.Queue));

        var factory = new ConnectionFactory()
        {
            HostName = _rabbitMqConfig.HostName,
            UserName = _rabbitMqConfig.UserName,
            Password = _rabbitMqConfig.Password,
            Port = _rabbitMqConfig.Port
        };

        // 使用using确保资源释放
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        // 声明队列，防止队列不存在
        await channel.QueueDeclareAsync(
            queue: publisher.Queue,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        // 如果指定了Exchange，先声明Exchange并绑定队列
        if (!string.IsNullOrEmpty(publisher.Exchange))
        {
            await channel.ExchangeDeclareAsync(
                exchange: publisher.Exchange,
                type: string.IsNullOrEmpty(publisher.ExchangeType) ? "direct" : publisher.ExchangeType,
                durable: false,
                autoDelete: false,
                arguments: null);

            await channel.QueueBindAsync(
                queue: publisher.Queue,
                exchange: publisher.Exchange,
                routingKey: publisher.RoutingKey ?? "");
        }

        MqMessage mqMessage =
            new MqMessage(Snow.GetId(),publisher.MessageType, publisher.Body);

        var body = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(mqMessage));

        // 发送消息
        await channel.BasicPublishAsync(
            exchange: publisher.Exchange ?? "",
            routingKey: publisher.RoutingKey ?? publisher.Queue,
            body: body);
    }

    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="subscriber"></param>
    /// <param name="onReceived"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public async Task ReceiveAsync(MqSubscriber subscriber, Func<MqMessage, Task> onReceived)
    {
        // 检查参数
        if (subscriber == null)
            throw new ArgumentNullException(nameof(subscriber));
        if (string.IsNullOrWhiteSpace(subscriber.Queue))
            throw new ArgumentException("队列名称不能为空", nameof(subscriber.Queue));
        if (onReceived == null)
            throw new ArgumentNullException(nameof(onReceived));

        var factory = new ConnectionFactory()
        {
            HostName = _rabbitMqConfig.HostName,
            UserName = _rabbitMqConfig.UserName,
            Password = _rabbitMqConfig.Password,
            Port = _rabbitMqConfig.Port
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: subscriber.Queue,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            string message = Encoding.UTF8.GetString(body);
            var mqMessage = System.Text.Json.JsonSerializer.Deserialize<MqMessage>(message);
            if (mqMessage == null)
            {
                _logger.LogError("RabbitMQ消息反序列化失败");
                throw new BusinessException("RabbitMQ消息反序列化失败");
            }

            try
            {
                _logger.LogInformation(
                    $"RabbitMQ消息接收开始：\r\n消息id：{mqMessage.Id}\r\n队列：{subscriber.Queue}\r\n" +
                    $"\r\n交换机：{subscriber.Exchange}\r\n路由键：{subscriber.RoutingKey}\r\n消息体：{mqMessage.Body}" +
                    $"\r\n消息类型：{mqMessage.Type}");
                await onReceived(mqMessage);
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Source + " " + ex.Message);
                await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await channel.BasicConsumeAsync(
            queue: subscriber.Queue,
            autoAck: false,
            consumer: consumer);

        _ = Task.Run(async () =>
        {
            try
            {
                // 阻塞当前任务，保持连接和通道存活
                await Task.Delay(Timeout.Infinite);
            }
            finally
            {
                await channel.CloseAsync();
                await connection.CloseAsync();
            }
        });
    }
}