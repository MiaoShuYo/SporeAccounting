using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Message.Model;
using SP.Common.Message.Mq;
using SP.Common.Message.Mq.Model;
using SP.ConfigService.Service;

namespace SP.ConfigService.Mq;

/// <summary>
/// 用户配置默认币种消息消费者服务
/// </summary>
public class UserConfigDefaultCurrencyConsumerService : BackgroundService
{
    /// <summary>
    /// RabbitMQ消息处理
    /// </summary>
    private readonly RabbitMqMessage _rabbitMqMessage;

    /// <summary>
    /// 用户配置服务
    /// </summary>
    private readonly IConfigServer _configServer;
    
    /// <summary>
    /// 日志记录器
    /// </summary>
    private readonly ILogger<UserConfigDefaultCurrencyConsumerService> _logger;

    /// <summary>
    /// 配置
    /// </summary>
    private readonly IConfiguration _configuration;


    /// <summary>
    /// 用户配置默认币种消息消费者服务构造函数
    /// </summary>
    /// <param name="rabbitMqMessage"></param>
    /// <param name="logger"></param>
    /// <param name="configuration"></param>
    public UserConfigDefaultCurrencyConsumerService(RabbitMqMessage rabbitMqMessage, ILogger<UserConfigDefaultCurrencyConsumerService> logger,
        IConfiguration configuration)
    {
        _rabbitMqMessage = rabbitMqMessage;
        _logger = logger;
        _configuration = configuration;
    }


    /// <summary>
    /// 消费者服务
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        MqSubscriber subscriber = new MqSubscriber(MqExchange.UserConfigExchange,
            MqRoutingKey.UserConfigDefaultCurrencyRoutingKey, MqQueue.UserConfigQueue);
        await _rabbitMqMessage.ReceiveAsync(subscriber, async message =>
        {
            MqMessage mqMessage = message as MqMessage;
            if (mqMessage == null)
            {
                _logger.LogError("消息转换失败");
                throw new ArgumentNullException(nameof(mqMessage));
            }

            string userId = mqMessage.Body;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("用户ID不能为空");
                throw new BusinessException(nameof(userId));
            }

            _logger.LogInformation($"接收到用户配置默认币种消息，用户ID: {userId}");
            if (!long.TryParse(userId, out long parsedUserId))
            {
                _logger.LogError("用户ID格式错误");
                throw new BusinessException("用户ID格式错误");
            }

            // 设置用户默认币种，默认币种id从配置文件中获取
            string defaultCurrencyId = _configuration["DefaultCurrencyId"];
            _logger.LogInformation($"nacos中配置的默认币种ID: {defaultCurrencyId}");
            // 调用币种服务设置用户默认币种
            await _configServer.SetUserDefaultCurrencyAsync(parsedUserId,defaultCurrencyId);
        });
    }
}