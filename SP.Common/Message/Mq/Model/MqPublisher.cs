namespace SP.Common.Message.Model.Mq;

/// <summary>
/// mq生产者类
/// </summary>
public class MqPublisher
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="body"></param>
    /// <param name="exchange"></param>
    /// <param name="routingKey"></param>
    /// <param name="queue"></param>
    /// <param name="messageType"></param>
    /// <param name="exchangeType"></param>
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