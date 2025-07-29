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
    /// 发送邮件/短信消息交换机
    /// </summary>
    public const string EmailExchange = "message_exchange";
    
    /// <summary>
    /// 用户配置交换机
    /// </summary>
    public const string UserConfigExchange = "user_config_exchange";
}