using System.Net;
using System.Text.Json;
using SP.Common.Message.Email;
using SP.Common.Message.Model;
using SP.Common.Message.Mq;
using SP.Common.Message.Mq.Model;
using SP.Common.Message.SmS.Model;
using SP.Common.Model.Enumeration;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Mq.Models;
using SP.FinanceService.RefitClient;
using SP.FinanceService.Service;

namespace SP.FinanceService.Mq;

/// <summary>
/// 预算通知消息消费者服务
/// </summary>
public class BudgetNotificationConsumerService : BackgroundService
{
    /// <summary>
    /// RabbitMq 消息
    /// </summary>
    private readonly RabbitMqMessage _rabbitMqMessage;

    /// <summary>
    /// 日志记录器
    /// </summary>
    private readonly ILogger<BudgetNotificationConsumerService> _logger;

    /// <summary>
    /// 服务作用域工厂
    /// </summary>
    private readonly IServiceScopeFactory _serviceScopeFactory;

    /// <summary>
    /// 邮件服务
    /// </summary>
    private readonly EmailMessage _emailMessage;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="rabbitMqMessage">RabbitMQ消息服务</param>
    /// <param name="logger">日志记录器</param>
    /// <param name="serviceScopeFactory">服务作用域工厂</param>
    /// <param name="emailMessage">邮件服务</param>
    public BudgetNotificationConsumerService(
        RabbitMqMessage rabbitMqMessage,
        ILogger<BudgetNotificationConsumerService> logger,
        IServiceScopeFactory serviceScopeFactory,
        EmailMessage emailMessage)
    {
        _rabbitMqMessage = rabbitMqMessage;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _emailMessage = emailMessage;
    }

    /// <summary>
    /// 执行异步任务
    /// </summary>
    /// <param name="stoppingToken">取消令牌</param>
    protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
    {
        MqSubscriber subscriber = new MqSubscriber(
            MqExchange.BudgetExchange,
            MqRoutingKey.BudgetRoutingKey,
            MqQueue.BudgetQueue);

        await _rabbitMqMessage.ReceiveAsync(subscriber, async message =>
        {
            // 验证消息
            if (message is not MqMessage mqMessage)
            {
                _logger.LogError("消息转换失败");
                return;
            }

            // 只处理预算通知相关的消息
            if (mqMessage.Type != MessageType.BudgetWarning &&
                mqMessage.Type != MessageType.BudgetExhausted &&
                mqMessage.Type != MessageType.BudgetOverrun)
            {
                // 不是通知类型的消息，直接返回（可能是其他预算相关消息）
                return;
            }

            if (string.IsNullOrEmpty(mqMessage.Body))
            {
                _logger.LogError("消息体为空，无法处理预算通知");
                throw new InvalidOperationException("预算通知消息体为空");
            }

            // 反序列化消息
            BudgetNotificationMQ? notification = JsonSerializer.Deserialize<BudgetNotificationMQ>(mqMessage.Body);
            if (notification == null)
            {
                _logger.LogError("消息体反序列化失败，无法处理预算通知");
                throw new InvalidOperationException("预算通知消息体反序列化失败");
            }

            try
            {
                // 使用服务作用域工厂创建作用域并获取预算服务
                using var scope = _serviceScopeFactory.CreateScope();
                var budgetServer = scope.ServiceProvider.GetRequiredService<IBudgetServer>();
                var userServiceApi = scope.ServiceProvider.GetRequiredService<IUserServiceApi>();

                // 获取预算信息
                var budget = budgetServer.QueryById(notification.BudgetId, notification.UserId);
                if (budget == null)
                {
                    _logger.LogWarning($"未找到预算信息，预算ID: {notification.BudgetId}");
                    return;
                }

                // 根据消息类型和用户偏好发送通知
                await ProcessNotification(mqMessage.Type, notification, budget, userServiceApi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"处理预算通知消息失败: {mqMessage.Type}, 用户ID: {notification.UserId}, 预算ID: {notification.BudgetId}");
                throw;
            }
        }, stoppingToken);
    }

    /// <summary>
    /// 处理通知
    /// </summary>
    /// <param name="messageType">消息类型</param>
    /// <param name="notification">通知消息</param>
    /// <param name="budget">预算信息</param>
    /// <param name="userServiceApi">用户服务API</param>
    private async System.Threading.Tasks.Task ProcessNotification(string messageType, BudgetNotificationMQ notification,
        BudgetResponse budget, IUserServiceApi userServiceApi)
    {
        // 构建通知内容
        string subject = GetNotificationSubject(messageType);
        string content = GetNotificationContent(messageType, notification, budget);

        // 根据用户偏好发送通知
        NotificationEnum preference = (NotificationEnum)notification.MessagePreference;

        switch (preference)
        {
            case NotificationEnum.Email:
                await SendEmailNotification(userServiceApi, notification.UserId, subject, content);
                break;

            case NotificationEnum.SmS:
                await SendSmsNotification(userServiceApi, notification.UserId, content);
                break;

            case NotificationEnum.InApp:
                await SendInAppNotification(userServiceApi, notification.UserId, subject, content);
                break;

            default:
                _logger.LogWarning($"未知的通知偏好: {notification.MessagePreference}");
                break;
        }
    }

    /// <summary>
    /// 获取通知主题
    /// </summary>
    /// <param name="messageType">消息类型</param>
    private string GetNotificationSubject(string messageType)
    {
        return messageType switch
        {
            MessageType.BudgetWarning => "预算预警提醒",
            MessageType.BudgetExhausted => "预算耗尽通知",
            MessageType.BudgetOverrun => "预算超支警告",
            _ => "预算通知"
        };
    }

    /// <summary>
    /// 获取通知内容
    /// </summary>
    /// <param name="messageType">消息类型</param>
    /// <param name="notification">通知消息</param>
    /// <param name="budget">预算信息</param>
    private string GetNotificationContent(string messageType, BudgetNotificationMQ notification, BudgetResponse budget)
    {
        return messageType switch
        {
            MessageType.BudgetWarning =>
                $"您的预算「{budget.TransactionCategoryName}」已使用 {notification.UsagePercent:F2}%，" +
                $"剩余金额：{budget.Remaining:F2}，请注意控制支出。",

            MessageType.BudgetExhausted =>
                $"您的预算「{budget.TransactionCategoryName}」已用完，" +
                $"预算金额：{budget.Amount:F2}，请谨慎支出。",

            MessageType.BudgetOverrun =>
                $"您的预算「{budget.TransactionCategoryName}」已超支 {notification.OverrunPercent:F2}%，" +
                $"预算金额：{budget.Amount:F2}，当前剩余：{budget.Remaining:F2}，请及时调整。",

            _ => $"预算「{budget.TransactionCategoryName}」状态已更新"
        };
    }

    /// <summary>
    /// 发送邮件通知
    /// </summary>
    /// <param name="userServiceApi">用户服务API</param>
    /// <param name="userId">用户ID</param>
    /// <param name="subject">主题</param>
    /// <param name="content">内容</param>
    private async System.Threading.Tasks.Task SendEmailNotification(IUserServiceApi userServiceApi, long userId,
        string subject, string content)
    {
        try
        {
            // 调用用户服务获取用户邮箱
            var userResponse = await userServiceApi.GetUser(userId);

            if (userResponse == null || userResponse.StatusCode != HttpStatusCode.OK || userResponse.Content == null)
            {
                _logger.LogWarning($"无法获取用户信息: 用户ID={userId}");
                return;
            }

            var user = userResponse.Content;

            if (string.IsNullOrEmpty(user.Email))
            {
                _logger.LogWarning($"用户邮箱为空，无法发送邮件通知: 用户ID={userId}");
                return;
            }

            // 构建完整的HTML邮件内容
            string htmlContent = $@"
                <html>
                <body>
                    <h2>{subject}</h2>
                    <p>尊敬的 {user.UserName}，您好！</p>
                    <p>{content}</p>
                    <br/>
                    <p>这是系统自动发送的邮件，请勿回复。</p>
                    <p>SporeAccounting 财务管理系统</p>
                </body>
                </html>";

            // 发送邮件到邮件队列
            await SendToEmailQueue(user.Email, subject, htmlContent);

            _logger.LogInformation($"邮件通知已发送到队列: 用户ID={userId}, 邮箱={user.Email}, 主题={subject}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"发送邮件通知失败: 用户ID={userId}");
        }
    }

    /// <summary>
    /// 发送短信通知
    /// </summary>
    /// <param name="userServiceApi">用户服务API</param>
    /// <param name="userId">用户ID</param>
    /// <param name="content">内容</param>
    private async System.Threading.Tasks.Task SendSmsNotification(IUserServiceApi userServiceApi, long userId,
        string content)
    {
        try
        {
            // 调用用户服务获取用户手机号
            var userResponse = await userServiceApi.GetUser(userId);

            if (userResponse == null || userResponse.StatusCode != HttpStatusCode.OK || userResponse.Content == null)
            {
                _logger.LogWarning($"无法获取用户信息: 用户ID={userId}");
                return;
            }

            var user = userResponse.Content;

            if (string.IsNullOrEmpty(user.PhoneNumber))
            {
                _logger.LogWarning($"用户手机号为空，无法发送短信通知: 用户ID={userId}");
                return;
            }

            // 短信内容限制长度（一般短信限制70字符）
            string smsContent = content.Length > 70
                ? content.Substring(0, 67) + "..."
                : content;

            // 发送短信到短信队列
            await SendToSmsQueue(user.PhoneNumber, smsContent);

            _logger.LogInformation($"短信通知已发送到队列: 用户ID={userId}, 手机号={MaskPhoneNumber(user.PhoneNumber)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"发送短信通知失败: 用户ID={userId}");
        }
    }

    /// <summary>
    /// 发送应用内通知
    /// </summary>
    /// <param name="userServiceApi">用户服务API</param>
    /// <param name="userId">用户ID</param>
    /// <param name="subject">主题</param>
    /// <param name="content">内容</param>
    private async System.Threading.Tasks.Task SendInAppNotification(IUserServiceApi userServiceApi, long userId,
        string subject, string content)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var inSiteNotificationsService = scope.ServiceProvider.GetRequiredService<IInSiteNotificationsServiceApi>();
            SendInSiteNotificationRequest sendInSiteNotification = new SendInSiteNotificationRequest();
            sendInSiteNotification.UserId = userId;
            sendInSiteNotification.Title = subject;
            sendInSiteNotification.Content = content;
            var response = await inSiteNotificationsService.SendInSiteNotification(sendInSiteNotification);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"站内信发送失败：{response.Error}");
                await System.Threading.Tasks.Task.CompletedTask;
            }
            _logger.LogInformation($"应用内通知: 用户ID={userId}, 主题={subject}, 内容={content}");
            await System.Threading.Tasks.Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"发送应用内通知失败: 用户ID={userId}");
        }
    }

    /// <summary>
    /// 发送到邮件队列
    /// </summary>
    /// <param name="email">邮箱地址</param>
    /// <param name="subject">主题</param>
    /// <param name="content">内容</param>
    private async System.Threading.Tasks.Task SendToEmailQueue(string email, string subject, string content)
    {
        try
        {
            // 直接使用邮件服务发送
            await _emailMessage.SendEmailAsync(email, subject, content);
            _logger.LogInformation($"邮件已发送: {email}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"发送邮件到队列失败: Email={email}");
        }
    }

    /// <summary>
    /// 发送到短信队列
    /// </summary>
    /// <param name="phoneNumber">手机号</param>
    /// <param name="content">内容</param>
    private async System.Threading.Tasks.Task SendToSmsQueue(string phoneNumber, string content)
    {
        try
        {
            // 构建短信消息
            var smsMessage = new SmSMessage
            {
                PhoneNumber = phoneNumber,
                Message = content,
                Purpose = SmSPurposeEnum.Reminder
            };

            string body = JsonSerializer.Serialize(smsMessage);

            // 发送到短信队列
            MqPublisher publisher = new MqPublisher(
                body,
                MqExchange.MessageExchange,
                MqRoutingKey.SmsRoutingKey,
                MqQueue.SmSQueue,
                MessageType.SmSGeneral,
                ExchangeType.Direct);

            await _rabbitMqMessage.SendAsync(publisher);

            _logger.LogInformation($"短信已发送到队列: {MaskPhoneNumber(phoneNumber)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"发送短信到队列失败: PhoneNumber={MaskPhoneNumber(phoneNumber)}");
        }
    }

    /// <summary>
    /// 隐藏手机号中间4位
    /// </summary>
    /// <param name="phoneNumber">手机号</param>
    /// <returns>隐藏后的手机号</returns>
    private string MaskPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length < 11)
        {
            return phoneNumber;
        }

        return phoneNumber.Substring(0, 3) + "****" + phoneNumber.Substring(7);
    }
}