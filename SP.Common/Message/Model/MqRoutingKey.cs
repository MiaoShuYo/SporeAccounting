namespace SP.Common.Message.Model;

/// <summary>
/// 消息队列路由键
/// </summary>
public static class MqRoutingKey
{
    /// <summary>
    /// 预算消息路由键
    /// </summary>
    public const string BudgetRoutingKey = "budget_routing_key";

    /// <summary>
    /// 发送邮件消息路由键
    /// </summary>
    public const string EmailRoutingKey = "email_routing_key";

    /// <summary>
    /// 用户配置默认币种消息路由键
    /// </summary>
    public const string UserConfigDefaultCurrencyRoutingKey = "user_config_default_currency_routing_key";
}