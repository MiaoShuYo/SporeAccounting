namespace SP.Common.Message.Model;

/// <summary>
/// 消息队列交换机
/// </summary>
public class MqExchange
{
    /// <summary>
    /// 预算消息交换机
    /// </summary>
    public const string BudgetExchange = "budget_exchange";
    /// <summary>
    /// 发送邮件消息交换机
    /// </summary>
    public const string EmailExchange = "email_exchange";
}