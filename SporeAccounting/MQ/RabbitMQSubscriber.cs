using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SporeAccounting.MQ;

/// <summary>
/// RabbitMQ订阅者
/// </summary>
public class RabbitMQSubscriber
{
    /// <summary>
    /// RabbitMQ连接
    /// </summary>
    private readonly RabbitMQConnection _connection;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="connection"></param>
    public RabbitMQSubscriber(RabbitMQConnection connection)
    {
        _connection = connection;
    }

    /// <summary>
    /// 订阅
    /// </summary>
    /// <param name="queue"></param>
    /// <param name="routingKey"></param>
    /// <param name="onMessage"></param>
    /// <returns></returns>
    public async System.Threading.Tasks.Task Subscribe(string queue, string routingKey,
        Action<string> onMessage)
    {
        await using var channel = await _connection.CreateChannel();
        await channel.QueueDeclareAsync(queue, durable: false, exclusive: false, autoDelete: false,
            arguments: null);
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (sender, @event) =>
        {
            var body = @event.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            onMessage(message);
            return System.Threading.Tasks.Task.CompletedTask;
        };
        await channel.BasicConsumeAsync(queue, autoAck: true, consumer: consumer);
    }
}