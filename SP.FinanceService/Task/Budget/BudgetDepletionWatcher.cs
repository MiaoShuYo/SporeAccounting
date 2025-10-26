using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Refit;
using SP.Common.Message.Model;
using SP.Common.Message.Mq;
using SP.Common.Message.Mq.Model;
using SP.Common.Model.Enumeration;
using SP.Common.Redis;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Mq.Models;
using SP.FinanceService.RefitClient;
using SP.FinanceService.Service;

namespace SP.FinanceService.Task.Budget;

/// <summary>
/// 预算监控预警
/// </summary>
public class BudgetDepletionWatcher : IJob
{
    /// <summary>
    /// 预算服务
    /// </summary>
    private readonly IBudgetServer _budgetServer;

    /// <summary>
    /// 用户配置接口
    /// </summary>
    private readonly IConfigServiceApi _configService;

    /// <summary>
    /// Redis服务
    /// </summary>
    private readonly IRedisService _redisService;

    /// <summary>
    /// 日志记录器
    /// </summary>
    private readonly ILogger<BudgetDepletionWatcher> _logger;

    /// <summary>
    /// RabbitMQ消息服务
    /// </summary>
    private readonly RabbitMqMessage _rabbitMqMessage;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="budgetServer">预算服务</param>
    /// <param name="configService">用户配置接口</param>
    /// <param name="redisService">Redis服务</param>
    /// <param name="logger">日志记录器</param>
    /// <param name="rabbitMqMessage">RabbitMQ消息服务</param>
    public BudgetDepletionWatcher(
        IBudgetServer budgetServer,
        IConfigServiceApi configService,
        IRedisService redisService,
        ILogger<BudgetDepletionWatcher> logger,
        RabbitMqMessage rabbitMqMessage)
    {
        _budgetServer = budgetServer;
        _configService = configService;
        _redisService = redisService;
        _logger = logger;
        _rabbitMqMessage = rabbitMqMessage;
    }

    /// <summary>
    /// 预算监控预警
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async System.Threading.Tasks.Task Execute(IJobExecutionContext context)
    {
        var today = DateTime.Now.ToString("yyyyMMdd");
        var now = DateTime.Now;

        // Redis Hash Key，用于存储今日已通知的预算
        // 预警通知
        string depletionHashKey = string.Format(SPRedisKey.DepletionHashKey, today);
        // 耗尽通知
        string exhaustedHashKey = string.Format(SPRedisKey.ExhaustedHashKey, today);
        // 超额通知
        string overrunHashKey = string.Format(SPRedisKey.OverrunHashKey, today);
        var budgets = _budgetServer.QueryBudgetsByDate(now);
        // 流式查询所有当前生效的预算，避免一次性加载到内存
        await foreach (var budget in budgets.AsAsyncEnumerable())
        {
            try
            {
                // 计算预算使用情况
                decimal usedAmount = budget.Amount - budget.Remaining;
                decimal usagePercent = budget.Amount > 0
                    ? (usedAmount / budget.Amount) * 100
                    : 0;

                // 获取用户配置的预警阈值（默认80%）
                decimal warningThreshold = await GetUserWarningThreshold(budget.CreateUserId);
                // 获取用户配置的通知发送方式，默认站内信
                int messagePreference = await GetUserMessagePreference(budget.CreateUserId);

                // 1. 预算预警通知（达到阈值但未耗尽）
                if (usagePercent >= warningThreshold && budget.Remaining > 0)
                {
                    string? notifiedValue = await _redisService.HashGetAsync(
                        depletionHashKey,
                        budget.Id.ToString());

                    if (string.IsNullOrEmpty(notifiedValue))
                    {
                        // 发送预警通知
                        await SendDepletionWarning(budget.CreateUserId, budget.Id, usagePercent, messagePreference);

                        // 标记已通知
                        await _redisService.HashSetAsync(
                            depletionHashKey,
                            budget.Id.ToString(),
                            DateTime.Now.ToString("O"));
                    }
                }

                // 2. 预算耗尽通知（刚好用完或略超）
                else if (budget.Remaining <= 0 && usedAmount <= budget.Amount * 1.1m)
                {
                    string? notifiedValue = await _redisService.HashGetAsync(
                        exhaustedHashKey,
                        budget.Id.ToString());

                    if (string.IsNullOrEmpty(notifiedValue))
                    {
                        // 发送耗尽通知
                        await SendExhaustedNotification(budget.CreateUserId, budget.Id, messagePreference);

                        // 标记已通知
                        await _redisService.HashSetAsync(
                            exhaustedHashKey,
                            budget.Id.ToString(),
                            DateTime.Now.ToString("O"));
                    }
                }

                // 3. 预算超额通知（超过预算10%以上）
                else if (usedAmount > budget.Amount * 1.1m)
                {
                    string? notifiedValue = await _redisService.HashGetAsync(
                        overrunHashKey,
                        budget.Id.ToString());

                    if (string.IsNullOrEmpty(notifiedValue))
                    {
                        // 发送超额通知
                        decimal overrunPercent = ((usedAmount - budget.Amount) / budget.Amount) * 100;
                        await SendOverrunNotification(budget.CreateUserId, budget.Id, overrunPercent,
                            messagePreference);

                        // 标记已通知
                        await _redisService.HashSetAsync(
                            overrunHashKey,
                            budget.Id.ToString(),
                            DateTime.Now.ToString("O"));
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录错误但继续处理其他预算
                _logger.LogError($"处理预算 {budget.Id} 时出错: {ex.Message}");
            }
        }

        // 设置Hash的过期时间（24小时后自动清理）
        await _redisService.SetExpiryAsync(depletionHashKey, 86400);
        await _redisService.SetExpiryAsync(exhaustedHashKey, 86400);
        await _redisService.SetExpiryAsync(overrunHashKey, 86400);
    }

    /// <summary>
    /// 获取用户消息偏好设置
    /// </summary>
    /// <param name="userId">人员id</param>
    /// <returns></returns>
    private async Task<int> GetUserMessagePreference(long userId)
    {
        try
        {
            // 调用配置服务获取用户设置的阈值
            ApiResponse<ConfigResponse> config =
                await _configService.QueryByTypeAndUserId(ConfigTypeEnum.Notification, userId);
            if (config == null || config.StatusCode != HttpStatusCode.OK)
            {
                return (int)NotificationEnum.InApp;
            }

            return config.Content != null &&
                   int.TryParse(config.Content.Value, out var preference)
                ? preference
                : (int)NotificationEnum.InApp;
        }
        catch
        {
            return (int)NotificationEnum.InApp;
        }
    }

    /// <summary>
    /// 获取用户配置的预警阈值
    /// </summary>
    private async Task<decimal> GetUserWarningThreshold(long userId)
    {
        try
        {
            // 调用配置服务获取用户设置的阈值
            ApiResponse<ConfigResponse> config =
                await _configService.QueryByTypeAndUserId(ConfigTypeEnum.BudgetAlertThreshold, userId);
            if (config == null || config.StatusCode != HttpStatusCode.OK)
            {
                return 80m;
            }

            return config.Content != null &&
                   decimal.TryParse(config.Content.Value, out var threshold)
                ? threshold
                : 80m;
        }
        catch
        {
            return 80m;
        }
    }

    /// <summary>
    /// 发送预算预警通知
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="budgetId">预算ID</param>
    /// <param name="usagePercent">使用百分比</param>
    /// <param name="messagePreference">消息偏好</param>
    private async System.Threading.Tasks.Task SendDepletionWarning(long userId, long budgetId, decimal usagePercent,
        int messagePreference)
    {
        try
        {
            // 构建通知消息
            var notification = new BudgetNotificationMQ
            {
                UserId = userId,
                BudgetId = budgetId,
                UsagePercent = usagePercent,
                MessagePreference = messagePreference
            };

            string body = JsonSerializer.Serialize(notification);

            // 发送到MQ
            MqPublisher publisher = new MqPublisher(
                body,
                MqExchange.BudgetExchange,
                MqRoutingKey.BudgetRoutingKey,
                MqQueue.BudgetQueue,
                MessageType.BudgetWarning,
                ExchangeType.Direct);

            await _rabbitMqMessage.SendAsync(publisher);

            _logger.LogInformation($"预警通知已发送：用户 {userId} 的预算 {budgetId} 已使用 {usagePercent:F2}%");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"发送预算预警通知失败：用户 {userId}，预算 {budgetId}");
        }
    }

    /// <summary>
    /// 发送预算耗尽通知
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="budgetId">预算ID</param>
    /// <param name="messagePreference">消息偏好</param>
    private async System.Threading.Tasks.Task SendExhaustedNotification(long userId, long budgetId,
        int messagePreference)
    {
        try
        {
            // 构建通知消息
            var notification = new BudgetNotificationMQ
            {
                UserId = userId,
                BudgetId = budgetId,
                MessagePreference = messagePreference
            };

            string body = JsonSerializer.Serialize(notification);

            // 发送到MQ
            MqPublisher publisher = new MqPublisher(
                body,
                MqExchange.BudgetExchange,
                MqRoutingKey.BudgetRoutingKey,
                MqQueue.BudgetQueue,
                MessageType.BudgetExhausted,
                ExchangeType.Direct);

            await _rabbitMqMessage.SendAsync(publisher);

            _logger.LogInformation($"耗尽通知已发送：用户 {userId} 的预算 {budgetId} 已用完");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"发送预算耗尽通知失败：用户 {userId}，预算 {budgetId}");
        }
    }

    /// <summary>
    /// 发送预算超额通知
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="budgetId">预算ID</param>
    /// <param name="overrunPercent">超额百分比</param>
    /// <param name="messagePreference">消息偏好</param>
    private async System.Threading.Tasks.Task SendOverrunNotification(long userId, long budgetId,
        decimal overrunPercent, int messagePreference)
    {
        try
        {
            // 构建通知消息
            var notification = new BudgetNotificationMQ
            {
                UserId = userId,
                BudgetId = budgetId,
                OverrunPercent = overrunPercent,
                MessagePreference = messagePreference
            };

            string body = JsonSerializer.Serialize(notification);

            // 发送到MQ
            MqPublisher publisher = new MqPublisher(
                body,
                MqExchange.BudgetExchange,
                MqRoutingKey.BudgetRoutingKey,
                MqQueue.BudgetQueue,
                MessageType.BudgetOverrun,
                ExchangeType.Direct);

            await _rabbitMqMessage.SendAsync(publisher);

            _logger.LogInformation($"超额通知已发送：用户 {userId} 的预算 {budgetId} 已超支 {overrunPercent:F2}%");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"发送预算超额通知失败：用户 {userId}，预算 {budgetId}");
        }
    }
}