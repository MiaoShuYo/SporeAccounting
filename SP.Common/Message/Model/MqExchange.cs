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
    public const string MessageExchange = "message_exchange";
    
    /// <summary>
    /// 用户配置交换机
    /// </summary>
    public const string UserConfigExchange = "user_config_exchange";

    /// <summary>
    /// 账本分享交换机
    /// </summary>
    public const string AccountBookShareExchange = "account_book_share_exchange";

    /// <summary>
    /// 死信交换机
    /// </summary>
    public const string DeadLetterExchange = "dead_letter_exchange";
}