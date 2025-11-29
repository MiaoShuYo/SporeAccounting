namespace SP.Common.Redis;

/// <summary>
/// 通用Redis键类
/// </summary>
public class SPRedisKey
{
    /// <summary>
    /// 用户Token键
    /// </summary>
    public const string Token = "Token:{0}";

    /// <summary>
    /// 短信验证码键
    /// </summary>
    public const string SmSCode = "SMS:OTP:{0}:{1}";

    /// <summary>
    /// 短信发送限流键
    /// </summary>
    public const string SmsLimit = "SMS:OTP:LIMIT:{0}";

    /// <summary>
    /// 短信发送天限流
    /// </summary>
    public const string SmSLimitDay = "SMS:OTP:LIMIT:DAY:{0}";

    ///<summary>
    /// 图形验证码限流
    /// </summary>
    public const string CaptchaRateLimit = "CAPTCHA:RATE:{0}";

    /// <summary>
    /// 预警通知
    /// </summary>
    public const string DepletionHashKey = "BUDGET:NOTIFIED:DEPLETION:{0}";

    /// <summary>
    /// 耗尽通知
    /// </summary>
    public const string ExhaustedHashKey = "BUDGET:NOTIFIED:EXHAUSTED:{0}";

    /// <summary>
    /// 超额通知
    /// </summary>
    public const string OverrunHashKey = "BUDGET:NOTIFIED:OVERRUN:{0}";

    /// <summary>
    /// 预算提醒频率
    /// </summary>
    public const string ReminderFrequencyKey = "BUDGET:NOTIFIED:REMINDER:FREQUENCY:{0}:{1}";
}