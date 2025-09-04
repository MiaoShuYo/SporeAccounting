namespace SP.Common.Message.SmS.Model;

/// <summary>
/// Twilio 短信配置选项
/// </summary>
public class TwilioSmsOptions
{
    /// <summary>
    /// Twilio 账号 SID
    /// </summary>
    public string AccountSid { get; set; }

    /// <summary>
    /// Twilio 认证 Token
    /// </summary>
    public string AuthToken { get; set; }

    /// <summary>
    /// 发送短信的电话号码（必须是 Twilio 购买的号码）
    /// </summary>
    public string FromNumber { get; set; }
    
    /// <summary>
    /// 可选：使用 Messaging Service 发送短信时的服务 SID
    /// </summary>
    public string? MessagingServiceSid { get; set; }
    
    /// <summary>
    /// 验证码有效期，默认300秒（5分钟）
    /// </summary>
    public int CodeTTLSeconds { get; set; } = 300;
    
    /// <summary>
    /// 发送间隔，默认60秒（防止频繁发送）
    /// </summary>
    public int SendIntervalSeconds { get; set; } = 60;

    /// <summary>
    /// 短信签名
    /// </summary>
    public string Signature { get; set; } = "孢子记账";
    
    /// <summary>
    /// 每天发送次数限制，默认5次
    /// </summary>
    public int SendNumLimitPerDay { get; set; } = 5;
}