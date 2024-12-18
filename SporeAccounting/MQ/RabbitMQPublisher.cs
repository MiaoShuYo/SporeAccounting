using RabbitMQ.Client;

namespace SporeAccounting.MQ;

/// <summary>
/// RabbitMQ发布者类
/// </summary>
public class RabbitMQPublisher
{
    /// <summary>
    /// RabbitMQ连接
    /// </summary>
    private readonly RabbitMQConnection _connection;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="connection"></param>
    public RabbitMQPublisher(RabbitMQConnection connection)
    {
        _connection = connection;
    }

    /// <summary>
    /// 发布消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="queue"></param>
    /// <param name="routingKey"></param>
    /// <param name="message"></param>
    public async System.Threading.Tasks.Task Publish<T>(string queue, string routingKey, T message)
    {
        await using var channel = await _connection.CreateChannel();
        await channel.QueueDeclareAsync(queue, durable: true);
        var body = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(message));
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: routingKey, body: body);
    }
}