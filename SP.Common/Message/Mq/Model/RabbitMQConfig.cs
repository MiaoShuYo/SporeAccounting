namespace SP.Common.Message.Mq.Model;

/// <summary>
/// RabbitMQ配置类
/// </summary>
public class RabbitMqConfig
{
    /// <summary>
    /// RabbitMQ主机名/IP地址
    /// </summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = "guest";

    /// <summary>
    /// 密码
    ////</summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    /// 端口
    ///</summary>
    public int Port { get; set; } = 5672;
}