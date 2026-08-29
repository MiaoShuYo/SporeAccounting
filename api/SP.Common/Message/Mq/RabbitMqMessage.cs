using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Message.Model;
using SP.Common.Message.Mq.Model;

namespace SP.Common.Message.Mq;

/// <summary>
/// RabbitMQ消息类
/// </summary>
public class RabbitMqMessage
{
    private const string RetryCountHeader = "x-retry-count";
    private const int MaxRetryCount = 5;

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
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        // 如果指定了Exchange，先声明Exchange并绑定队列
        if (!string.IsNullOrEmpty(publisher.Exchange))
        {
            await channel.ExchangeDeclareAsync(
                exchange: publisher.Exchange,
                type: string.IsNullOrEmpty(publisher.ExchangeType) ? "direct" : publisher.ExchangeType,
                durable: true,
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

        var properties = new BasicProperties
        {
            Persistent = true,
            Headers = new Dictionary<string, object?>()
        };
        properties.Headers[RetryCountHeader] = 0;

        // 发送消息
        await channel.BasicPublishAsync(
            exchange: publisher.Exchange ?? "",
            routingKey: publisher.RoutingKey ?? publisher.Queue,
            mandatory: false,
            basicProperties: properties,
            body: body);
    }

    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="subscriber"></param>
    /// <param name="onReceived"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public async Task ReceiveAsync(MqSubscriber subscriber, Func<MqMessage, Task> onReceived,
        CancellationToken cancellationToken = default)
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
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                var mqMessage = System.Text.Json.JsonSerializer.Deserialize<MqMessage>(message);
                if (mqMessage == null)
                {
                    _logger.LogError("RabbitMQ消息反序列化失败，消息将被丢弃，DeliveryTag={DeliveryTag}", ea.DeliveryTag);
                    await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    return;
                }

                // 为防止敏感信息外泄，不记录 Exchange、Queue、RoutingKey 和消息体内容
                _logger.LogInformation(
                    $"RabbitMQ消息接收开始：\r\n消息id：{mqMessage.Id}\r\n消息类型：{mqMessage.Type}");
                await onReceived(mqMessage);
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Source + " " + ex.Message);
                var retryCount = GetRetryCount(ea);
                if (retryCount >= MaxRetryCount)
                {
                    _logger.LogError("RabbitMQ消息处理失败已超最大重试次数，消息将被丢弃。DeliveryTag={DeliveryTag}, RetryCount={RetryCount}",
                        ea.DeliveryTag, retryCount);
                    await PublishToDeadLetterAsync(channel, ea, retryCount);
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    return;
                }

                var nextRetryCount = retryCount + 1;
                await RepublishWithRetryCountAsync(channel, ea, nextRetryCount);
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
        };

        await channel.BasicConsumeAsync(
            queue: subscriber.Queue,
            autoAck: false,
            consumer: consumer);

        try
        {
            // 阻塞当前任务，保持连接和通道存活，直到宿主停止
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("RabbitMQ消费者收到停止信号，正在关闭连接。Queue={Queue}", subscriber.Queue);
        }
        finally
        {
            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }

    private static int GetRetryCount(BasicDeliverEventArgs ea)
    {
        if (ea.BasicProperties?.Headers == null)
        {
            return 0;
        }

        if (!ea.BasicProperties.Headers.TryGetValue(RetryCountHeader, out var headerValue) || headerValue == null)
        {
            return 0;
        }

        return headerValue switch
        {
            byte[] bytes when int.TryParse(Encoding.UTF8.GetString(bytes), out var parsed) => parsed,
            int intValue => intValue,
            long longValue => (int)longValue,
            _ => 0
        };
    }

    private static async Task RepublishWithRetryCountAsync(IChannel channel, BasicDeliverEventArgs ea, int retryCount)
    {
        var republishProperties = new BasicProperties
        {
            Persistent = true,
            Headers = new Dictionary<string, object?>
            {
                [RetryCountHeader] = retryCount
            }
        };

        await channel.BasicPublishAsync(
            exchange: ea.Exchange,
            routingKey: ea.RoutingKey,
            mandatory: false,
            basicProperties: republishProperties,
            body: ea.Body);
    }

    private static async Task PublishToDeadLetterAsync(IChannel channel, BasicDeliverEventArgs ea, int retryCount)
    {
        await channel.QueueDeclareAsync(
            queue: MqQueue.DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        await channel.ExchangeDeclareAsync(
            exchange: MqExchange.DeadLetterExchange,
            type: RabbitMQ.Client.ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            arguments: null);

        await channel.QueueBindAsync(
            queue: MqQueue.DeadLetterQueue,
            exchange: MqExchange.DeadLetterExchange,
            routingKey: MqRoutingKey.DeadLetterRoutingKey);

        var deadLetterProperties = new BasicProperties
        {
            Persistent = true,
            Headers = new Dictionary<string, object?>
            {
                [RetryCountHeader] = retryCount,
                ["x-original-exchange"] = ea.Exchange,
                ["x-original-routing-key"] = ea.RoutingKey
            }
        };

        await channel.BasicPublishAsync(
            exchange: MqExchange.DeadLetterExchange,
            routingKey: MqRoutingKey.DeadLetterRoutingKey,
            mandatory: false,
            basicProperties: deadLetterProperties,
            body: ea.Body);
    }
}