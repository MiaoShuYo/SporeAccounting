namespace SP.Common.Message.Mq.Model;

/// <summary>
/// mq消息类
/// </summary>
public class MqMessage
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <param name="body"></param>
    public MqMessage(long id, string type, string body)
    {
        Id = id;
        Body = body;
    }

    /// <summary>
    /// 消息ID
    /// </summary>
    public long Id { get; }

    /// <summary>
    /// 消息类型
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// 消息主题
    /// </summary>
    public string Body { get; }
}