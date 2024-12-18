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
    public async System.Threading.Tasks.Task Subscribe<T>(string queue, string routingKey,
        Action<T> onMessage)
    {
        // 创建一个新的通道
        await using var channel = await _connection.CreateChannel();

        // 声明一个队列
        await channel.QueueDeclareAsync(queue, durable: false, exclusive: false, autoDelete: false,
            arguments: null);

        // 创建一个异步事件消费者
        var consumer = new AsyncEventingBasicConsumer(channel);

        // 处理接收到的消息
        consumer.ReceivedAsync += (sender, @event) =>
        {
            // 获取消息体
            var body = @event.Body.ToArray();

            // 将消息体转换为字符串
            var message = Encoding.UTF8.GetString(body);

            // 反序列化消息
            var deserializedMessage = System.Text.Json.JsonSerializer.Deserialize<T>(message);

            // 调用消息处理委托
            onMessage(deserializedMessage);

            return System.Threading.Tasks.Task.CompletedTask;
        };

        // 开始消费队列中的消息
        await channel.BasicConsumeAsync(queue, autoAck: true, consumer: consumer);
    }
}