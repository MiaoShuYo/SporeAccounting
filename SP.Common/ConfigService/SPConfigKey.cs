namespace SP.Common.ConfigService;

/// <summary>
/// 配置key
/// </summary>
public class SPConfigKey
{
    /// <summary>
    /// 图形验证码限流开关
    /// </summary>
    public const string CaptchaRateLimitEnabled = "Captcha:RateLimit:Enabled";

    /// <summary>
    /// 图形验证码窗口时间
    /// </summary>
    public const string CaptchaRateLimitWindowSeconds = "Captcha:RateLimit:WindowSeconds";

    /// <summary>
    /// 图形验证码窗口时间内最大请求次数
    /// </summary>
    public const string CaptchaRateLimitMaxRequests = "Captcha:RateLimit:MaxRequests";
    
    /// <summary>
    /// 图形验证码宽度
    /// </summary>
    public const string CaptchaWidth = "Captcha:Width";
    
    /// <summary>
    /// 图形验证码高度
    /// </summary>
    public const string CaptchaHeight = "Captcha:Height";
    
    /// <summary>
    /// 图形验证码长度
    /// </summary>
    public const string CaptchaLength = "Captcha:Length";
    
    /// <summary>
    /// 图形验证码有效期
    /// </summary>
    public const string CaptchaExpirySeconds = "Captcha:ExpiresSeconds";
}