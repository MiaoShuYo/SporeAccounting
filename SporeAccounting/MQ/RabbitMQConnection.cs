using RabbitMQ.Client;
using SporeAccounting.MQ.Model;

namespace SporeAccounting.MQ;

/// <summary>
/// RabbitMQ连接类
/// </summary>
public class RabbitMQConnection : IDisposable
{
    /// <summary>
    /// 连接
    /// </summary>
    private readonly IConnection _connection;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options"></param>
    public RabbitMQConnection(RabbitMQOptions options)
    {
        var factory = new ConnectionFactory
        {
            HostName = options.HostName,
            Port = options.Port,
            VirtualHost = options.VirtualHost,
            UserName = options.UserName,
            Password = options.Password
        };
        _connection = factory.CreateConnectionAsync().Result;
    }

    /// <summary>
    /// 创建通道
    /// </summary>
    /// <returns></returns>
    public async Task<IChannel> CreateChannel()
    {
        return await _connection.CreateChannelAsync();
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose() => _connection.Dispose();
}