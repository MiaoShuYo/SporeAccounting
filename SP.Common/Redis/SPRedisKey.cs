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
    
    public const string SmSLimitDay ="SMS:OTP:LIMIT:DAY:{0}";
}