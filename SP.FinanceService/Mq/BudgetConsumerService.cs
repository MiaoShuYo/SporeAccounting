using SP.Common;
using SP.Common.Message.Model;
using SP.Common.Message.Mq;
using SP.Common.Message.Mq.Model;
using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Enumeration;
using SP.FinanceService.Models.Response;
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
    /// 预算服务
    /// </summary>
    private readonly IBudgetServer _budgetService;

    /// <summary>
    ///  Budget 消息消费者服务
    /// </summary>
    /// <param name="rabbitMqMessage"></param>
    /// <param name="logger"></param>
    /// <param name="budgetService"></param>
    public BudgetConsumerService(RabbitMqMessage rabbitMqMessage, ILogger<BudgetConsumerService> logger,
        IBudgetServer budgetService)
    {
        _logger = logger;
        _rabbitMqMessage = rabbitMqMessage;
        _budgetService = budgetService;
    }

    /// <summary>
    /// RabbitMq 消息
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        MqSubscriber subscriber = new MqSubscriber(MqExchange.BudgetExchange,
            MqRoutingKey.BudgetRoutingKey, MqQueue.BudgetQueue);
        await _rabbitMqMessage.ReceiveAsync(subscriber, async message =>
        {
            // 验证消息
            MqMessage mqMessage = ValidateMessage(message);
            if (mqMessage == null) return;

            // 获取当前预算
            List<Budget> budgets = GetCurrentBudgets();
            if (budgets == null || budgets.Count == 0) return;

            decimal amount = decimal.Parse(message.Body);

            // 根据消息类型处理预算
            switch (mqMessage.Type)
            {
                case MessageType.BudgetAdd:
                    _logger.LogInformation("接收到预算增加处理消息， {Message}", mqMessage);
                    UpdateBudgetsByAmount(budgets, amount, true);
                    break;
                case MessageType.BudgetUpdate:
                    _logger.LogInformation("接收到预算更新消息， {Message}", mqMessage);
                    UpdateBudgetsByAmount(budgets, amount, true);
                    break;
                case MessageType.BudgetDeduct:
                    _logger.LogInformation("接收到预算扣除消息， {Message}", mqMessage);
                    UpdateBudgetsByAmount(budgets, amount, false);
                    break;
                default:
                    _logger.LogWarning("未知的消息类型: {Type}", mqMessage.Type);
                    break;
            }

            await Task.CompletedTask;
        });
    }

    /// <summary>
    /// 验证消息
    /// </summary>
    /// <param name="message">原始消息</param>
    /// <returns>验证后的 MqMessage，如果验证失败返回 null</returns>
    private MqMessage ValidateMessage(object message)
    {
        MqMessage mqMessage = message as MqMessage;
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
    /// <returns>当前预算列表，如果没有可用预算返回空列表</returns>
    private List<Budget> GetCurrentBudgets()
    {
        List<Budget> budgets = _budgetService.QueryCurrentBudgets();
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
    /// <param name="budgets">预算列表</param>
    /// <param name="amount">金额</param>
    /// <param name="isAdd">是否为增加操作，true为增加，false为扣除</param>
    private void UpdateBudgetsByAmount(List<Budget> budgets, decimal amount, bool isAdd)
    {
        string operation = isAdd ? "增加" : "扣除";
        decimal operationAmount = isAdd ? amount : -amount;

        // 更新月度预算
        UpdateBudgetByPeriod(budgets, PeriodEnum.Month, operationAmount, operation);
        
        // 更新年度预算
        UpdateBudgetByPeriod(budgets, PeriodEnum.Year, operationAmount, operation);
        
        // 更新季度预算
        UpdateBudgetByPeriod(budgets, PeriodEnum.Quarter, operationAmount, operation);

        // 保存更改到数据库
        _budgetService.UpdateBudgets(budgets);
    }

    /// <summary>
    /// 根据周期更新预算
    /// </summary>
    /// <param name="budgets">预算列表</param>
    /// <param name="period">预算周期</param>
    /// <param name="amount">金额变化</param>
    /// <param name="operation">操作类型描述</param>
    private void UpdateBudgetByPeriod(List<Budget> budgets, PeriodEnum period, decimal amount, string operation)
    {
        Budget budget = budgets.FirstOrDefault(b => b.Period == period);
        if (budget != null)
        {
            budget.Amount += amount;
            string periodName = GetPeriodName(period);
            _logger.LogInformation("{PeriodName}预算{Operation}成功，{Operation}金额: {Amount}", 
                periodName, operation, operation, budget.Amount);
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