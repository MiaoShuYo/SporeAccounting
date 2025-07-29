namespace SP.Common.Message.Mq.Model;

/// <summary>
/// mq生产者类
/// </summary>
public class MqPublisher
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="body">消息体</param>
    /// <param name="exchange">交换机名称</param>
    /// <param name="routingKey">路由键</param>
    /// <param name="queue">队列</param>
    /// <param name="messageType">消息类型</param>
    /// <param name="exchangeType">ExchangeType</param>
    public MqPublisher(string body, string exchange, string routingKey, string queue, string messageType,
        string exchangeType)
    {
        Body = body;
        Exchange = exchange;
        RoutingKey = routingKey;
        Queue = queue;
        MessageType = messageType;
        ExchangeType = exchangeType;
    }

    /// <summary>
    /// 消息体
    /// </summary>
    public string Body { get; }

    /// <summary>
    /// 交换机名称
    /// </summary>
    public string Exchange { get; }

    /// <summary>
    /// 路由键
    /// </summary>
    public string RoutingKey { get; }

    /// <summary>
    /// 队列
    /// </summary>
    public string Queue { get; }

    /// <summary>
    /// 消息类型
    /// </summary>
    public string MessageType { get; }

    /// <summary>
    /// 交换机类型
    /// </summary>
    public string ExchangeType { get; }
}