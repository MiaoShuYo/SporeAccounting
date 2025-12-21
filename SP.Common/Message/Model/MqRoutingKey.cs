namespace SP.Common.Message.Model;

/// <summary>
/// 消息队列路由键
/// </summary>
public static class MqRoutingKey
{
    /// <summary>
    /// OCR消息路由键
    /// </summary>
    public const string OCRRoutingKey = "ocr_routing_key";

    /// <summary>
    /// 预算消息路由键
    /// </summary>
    public const string BudgetRoutingKey = "budget_routing_key";

    /// <summary>
    /// 邮件消息路由键（推荐）
    /// </summary>
    public const string EmailRoutingKey = "email_routing_key";

    /// <summary>
    /// 短信消息路由键（推荐）
    /// </summary>
    public const string SmsRoutingKey = "sms_routing_key";

    /// <summary>
    /// 用户配置默认币种消息路由键
    /// </summary>
    public const string UserConfigDefaultCurrencyRoutingKey = "user_config_default_currency_routing_key";

    /// <summary>
    /// 账本分享
    /// </summary>
    public const string AccountBookShareRoutingKey = "account_book_share_routing_key";
}