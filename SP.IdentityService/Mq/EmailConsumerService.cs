using SP.Common;
using SP.Common.Message.Email;
using SP.Common.Message.Model;
using SP.Common.Message.Mq;
using SP.Common.Message.Mq.Model;
using SP.Common.Redis;

namespace SP.IdentityService.Mq;

/// <summary>
/// Email 消息消费者服务
/// </summary>
public class EmailConsumerService : BackgroundService
{
    /// <summary>
    /// RabbitMq 消息
    /// </summary>
    private readonly RabbitMqMessage _rabbitMqMessage;

    /// <summary>
    /// 日志记录器
    /// </summary>
    private readonly ILogger<EmailConsumerService> _logger;

    /// <summary>
    /// 邮件发送
    /// </summary>
    private readonly EmailMessage _emailMessage;

    /// <summary>
    /// Redis 服务
    /// </summary>
    private readonly IRedisService _redis;

    /// <summary>
    /// Email 消息消费者服务构造函数
    /// </summary>
    /// <param name="rabbitMqMessage"></param>
    /// <param name="emailMessage"></param>
    /// <param name="logger"></param>
    /// <param name="redis"></param>
    public EmailConsumerService(RabbitMqMessage rabbitMqMessage,
        EmailMessage emailMessage,
        ILogger<EmailConsumerService> logger, IRedisService redis)
    {
        _logger = logger;
        _rabbitMqMessage = rabbitMqMessage;
        _emailMessage = emailMessage;
        _redis = redis;
    }

    /// <summary>
    /// 执行异步任务
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <exception cref="NotImplementedException"></exception>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        MqSubscriber subscriber = new MqSubscriber("message",
            "email", "email");
        await _rabbitMqMessage.ReceiveAsync(subscriber, async message =>
        {
            MqMessage mqMessage = message as MqMessage;
            if (mqMessage == null)
            {
                _logger.LogError("消息转换失败");
                throw new ArgumentNullException(nameof(mqMessage));
            }

            string email = mqMessage.Body;
            string subject = "";
            if (mqMessage.Type == MessageType.VerifyEmail)
            {
                subject = "邮箱验证";
            }
            else if (mqMessage.Type == MessageType.ResetPassword)
            {
                subject = "重置密码";
            }
            else
            {
                _logger.LogError("消息类型错误");
                throw new ArgumentException("消息类型错误", nameof(mqMessage.Type));
            }

            string code = CodeGeneratorCommon.GenerateVerificationCode(6);
            // 写入redis
            await _redis.SetStringAsync(email, code, 60 * 5);
            // 发送邮件
            await _emailMessage.SendEmailAsync(email,
                subject,
                $"您的验证码是：{code} 五分钟内有效，请勿泄露给他人。");
            await Task.CompletedTask;
        });
    }
}