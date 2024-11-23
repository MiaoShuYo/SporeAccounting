namespace SporeAccounting.MQ.Model;

/// <summary>
/// RabbitMQ配置选项
/// </summary>
public class RabbitMQOptions
{
    /// <summary>
    /// 链接地址
    /// </summary>
    public string HostName { get; set; } = "localhost";
    /// <summary>
    /// 端口
    /// </summary>
    public int Port { get; set; } = 5672;
    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = "guest";
    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = "guest";
    /// <summary>
    /// 
    /// </summary>
    public string VirtualHost { get; set; } = "/";
}