namespace SP.Common.Message.Model;

public static class MqQueue
{
    /// <summary>
    /// OCR消息队列
    /// </summary>
    public const string OCRQueue="ocr_queue";

    /// <summary>
    /// 预算消息队列
    /// </summary>
    public const string BudgetQueue = "budget_queue";

    /// <summary>
    /// 发送邮件消息队列
    /// </summary>
    public const string EmailQueue = "email_queue";
    
    /// <summary>
    /// 发送短信消息队列
    /// </summary>
    public const string SmSQueue = "sms_queue";
    
    /// <summary>
    /// 用户配置消息队列
    /// </summary>
    public const string UserConfigQueue = "user_config_queue";

    /// <summary>
    /// 账本分享消息队列
    /// </summary>
    public const string AccountBookShareQueue = "account_book_share_queue";

    /// <summary>
    /// 死信消息队列
    /// </summary>
    public const string DeadLetterQueue = "dead_letter_queue";

}