using Microsoft.Extensions.Configuration;
using SP.Common.Message.Mq.Model;

namespace SP.Common.ConfigService;

/// <summary>
/// RabbitMQ配置服务
/// </summary>
public class RabbitMqConfigService
{
    private readonly IConfiguration _configuration;
    
    /// <summary>
    /// RabbitMQ配置服务构造函数
    /// </summary>
    /// <param name="configuration"></param>
    public RabbitMqConfigService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public RabbitMqConfig GetRabbitMqConfig()
    {
        var rabbitMqConfig = new RabbitMqConfig();
        _configuration.GetSection("RabbitMQConfig").Bind(rabbitMqConfig);
        return rabbitMqConfig;
    }
}