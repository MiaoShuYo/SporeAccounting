namespace SP.Common.Message.Model;

public static class MqQueue
{
    /// <summary>
    /// 预算消息队列
    /// </summary>
    public const string BudgetQueue = "budget_queue";

    /// <summary>
    /// 发送邮件/短信消息队列
    /// </summary>
    public const string EmailQueue = "message_queue";
    
    /// <summary>
    /// 用户配置消息队列
    /// </summary>
    public const string UserConfigQueue = "user_config_queue";
}