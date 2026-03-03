using System.Text.Json;
using SP.Common.Message.Model;
using SP.Common.Message.Mq;
using SP.Common.Message.Mq.Model;
using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Enumeration;
using SP.FinanceService.Mq.Models;
using SP.FinanceService.Service;

namespace SP.FinanceService.Mq;

/// <summary>
/// Budget 消息消费者服务
/// </summary>
public class BudgetConsumerService : BackgroundService
{
    /// <summary>
    /// RabbitMq 消息
    /// </summary>
    private readonly RabbitMqMessage _rabbitMqMessage;

    /// <summary>
    /// 日志记录器
    /// </summary>
    private readonly ILogger<BudgetConsumerService> _logger;

    /// <summary>
    /// 服务作用域工厂
    /// </summary>
    private readonly IServiceScopeFactory _serviceScopeFactory;

    /// <summary>
    ///  Budget 消息消费者服务
    /// </summary>
    /// <param name="rabbitMqMessage"></param>
    /// <param name="logger"></param>
    /// <param name="serviceScopeFactory"></param>
    public BudgetConsumerService(RabbitMqMessage rabbitMqMessage, ILogger<BudgetConsumerService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _rabbitMqMessage = rabbitMqMessage;
        _serviceScopeFactory = serviceScopeFactory;
    }

    /// <summary>
    /// RabbitMq 消息
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
    {
        MqSubscriber subscriber = new MqSubscriber(MqExchange.BudgetExchange,
            MqRoutingKey.BudgetRoutingKey, MqQueue.BudgetQueue);
        await _rabbitMqMessage.ReceiveAsync(subscriber, async message =>
        {
            // 验证消息
            MqMessage? mqMessage = ValidateMessage(message);
            if (mqMessage == null) return;

            if (string.IsNullOrEmpty(message.Body))
            {
                _logger.LogError("消息体为空，无法处理预算");
                return;
            }

            BudgetChangeMQ? bugChange = JsonSerializer.Deserialize<BudgetChangeMQ>(message.Body);
            if (bugChange == null)
            {
                _logger.LogError("消息体反序列化失败，无法处理预算");
                return;
            }

            // 使用服务作用域工厂创建作用域并获取预算服务
            using var scope = _serviceScopeFactory.CreateScope();
            var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetServer>();

            // 获取当前预算
            List<Budget> budgets = GetCurrentBudgets(budgetService, bugChange.TransactionCategoryId, bugChange.UserId);
            if (budgets == null || budgets.Count == 0) return;

            // 根据消息类型处理预算
            switch (mqMessage.Type)
            {
                case MessageType.BudgetAdd:
                    _logger.LogInformation("接收到预算增加处理消息， {Message}", mqMessage);
                    UpdateBudgetsByAmount(budgetService, budgets, bugChange, true);
                    break;
                case MessageType.BudgetUpdate:
                    _logger.LogInformation("接收到预算更新消息， {Message}", mqMessage);
                    UpdateBudgetsByAmount(budgetService, budgets, bugChange, true);
                    break;
                case MessageType.BudgetDeduct:
                    _logger.LogInformation("接收到预算扣除消息， {Message}", mqMessage);
                    UpdateBudgetsByAmount(budgetService, budgets, bugChange, false);
                    break;
                default:
                    _logger.LogWarning("未知的消息类型: {Type}", mqMessage.Type);
                    break;
            }

            await System.Threading.Tasks.Task.CompletedTask;
        }, stoppingToken);
    }

    /// <summary>
    /// 验证消息
    /// </summary>
    /// <param name="message">原始消息</param>
    /// <returns>验证后的 MqMessage，如果验证失败返回 null</returns>
    private MqMessage? ValidateMessage(object message)
    {
        MqMessage? mqMessage = message as MqMessage;
        if (mqMessage == null)
        {
            _logger.LogError("消息转换失败");
            return null;
        }

        return mqMessage;
    }

    /// <summary>
    /// 获取当前预算
    /// </summary>
    /// <param name="budgetService">预算服务</param>
    /// <param name="transactionCategoryId">收支分类ID</param>
    /// <param name="userId">用户ID</param>
    /// <returns>当前预算列表，如果没有可用预算返回空列表</returns>
    private List<Budget> GetCurrentBudgets(IBudgetServer budgetService, long transactionCategoryId, long userId)
    {
        List<Budget> budgets = budgetService.QueryCurrentBudgetsByExpenseCategoryId(transactionCategoryId, userId);
        if (budgets == null || budgets.Count == 0)
        {
            _logger.LogInformation("当前没有可用的预算，不执行处理");
            return new List<Budget>();
        }

        return budgets;
    }

    /// <summary>
    /// 根据金额更新预算
    /// </summary>
    /// <param name="budgetService">预算服务</param>
    /// <param name="budgets">预算列表</param>
    /// <param name="budgetChangeMq">金额</param>
    /// <param name="isAdd">是否为增加操作，true为增加，false为扣除</param>
    private void UpdateBudgetsByAmount(IBudgetServer budgetService, List<Budget> budgets, BudgetChangeMQ budgetChangeMq, bool isAdd)
    {
        string operation = isAdd ? "增加" : "扣除";
        decimal operationAmount = isAdd ? budgetChangeMq.ChangeAmount : -budgetChangeMq.ChangeAmount;

        // 更新月度预算
        UpdateBudgetByPeriod(budgets, PeriodEnum.Month, operationAmount, operation);

        // 更新年度预算
        UpdateBudgetByPeriod(budgets, PeriodEnum.Year, operationAmount, operation);

        // 更新季度预算
        UpdateBudgetByPeriod(budgets, PeriodEnum.Quarter, operationAmount, operation);

        // 保存更改到数据库
        budgetService.UpdateBudgets(budgets);
    }

    /// <summary>
    /// 根据周期更新预算
    /// </summary>
    /// <param name="budgets">预算列表</param>
    /// <param name="period">预算周期</param>
    /// <param name="operationAmount">金额变化</param>
    /// <param name="operation">操作类型描述</param>
    private void UpdateBudgetByPeriod(List<Budget> budgets, PeriodEnum period, decimal operationAmount, string operation)
    {
        Budget? budget = budgets.FirstOrDefault(b => b.Period == period);
        if (budget != null)
        {
            budget.Remaining += operationAmount;
            string periodName = GetPeriodName(period);
            _logger.LogInformation("{PeriodName}预算{Operation}成功，{Operation}金额: {Amount}",
                periodName, operation, operation, operationAmount);
        }
    }

    /// <summary>
    /// 获取周期名称
    /// </summary>
    /// <param name="period">预算周期</param>
    /// <returns>周期名称</returns>
    private string GetPeriodName(PeriodEnum period)
    {
        return period switch
        {
            PeriodEnum.Month => "月度",
            PeriodEnum.Year => "年度",
            PeriodEnum.Quarter => "季度",
            _ => "未知"
        };
    }
}