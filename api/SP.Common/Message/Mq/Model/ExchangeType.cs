namespace SP.Common.Message.Mq.Model;

/// <summary>
/// 消息交换机类型
/// </summary>
public static class ExchangeType
{
    /// <summary>
    /// 直连交换机
    /// </summary>
    public const string Direct = "direct";

    /// <summary>
    /// 扇形交换机
    /// </summary>
    public const string Fanout = "fanout";

    /// <summary>
    /// 主题交换机
    /// </summary>
    public const string Topic = "topic";

    /// <summary>
    /// Headers交换机
    /// </summary>
    public const string Headers = "headers";
}