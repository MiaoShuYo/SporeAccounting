using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SporeAccounting.MQ
{
    public class RabbitMQSubscriberService
    {
        private readonly RabbitMQConnection _connection;
        private readonly ILogger<RabbitMQSubscriberService> _logger;

        public RabbitMQSubscriberService(RabbitMQConnection connection, ILogger<RabbitMQSubscriberService> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        /// <summary>
        /// 订阅消息队列
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="queue">队列名称</param>
        /// <param name="routingKey">路由键</param>
        /// <param name="onMessage">处理消息的逻辑</param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task SubscribeAsync<T>(string queue, string routingKey, Action<T> onMessage)
        {
            var channel = await _connection.CreateChannel();

            // 声明队列
            await channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);

            // 创建消费者
            var consumer = new AsyncEventingBasicConsumer(channel);

            // 绑定接收事件
            consumer.ReceivedAsync += async (sender, @event) =>
            {
                try
                {
                    var body = @event.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    _logger.LogInformation($"Message received from queue '{queue}': {message}");

                    // 反序列化并调用处理逻辑
                    var deserializedMessage = System.Text.Json.JsonSerializer.Deserialize<T>(message);
                    onMessage(deserializedMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing message from queue '{queue}'.");
                }

                await System.Threading.Tasks.Task.CompletedTask;
            };

            // 开始消费队列
            await channel.BasicConsumeAsync(queue: queue, autoAck: true, consumer: consumer);
            _logger.LogInformation($"Subscribed to queue '{queue}' with routing key '{routingKey}'.");
        }
    }
}
