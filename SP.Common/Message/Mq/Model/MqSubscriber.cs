namespace SP.Common.Message.Model.Mq;

/// <summary>
/// mq订阅者类
/// </summary>
public class MqSubscriber
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="exchange"></param>
    /// <param name="routingKey"></param>
    /// <param name="queue"></param>
    public MqSubscriber(string exchange, string routingKey, string queue)
    {
        Exchange = exchange;
        RoutingKey = routingKey;
        Queue = queue;
    }

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
}