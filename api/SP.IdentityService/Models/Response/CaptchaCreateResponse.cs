namespace SP.IdentityService.Models.Response;

/// <summary>
/// 生成图形验证码响应
/// </summary>
public class CaptchaCreateResponse
{
    /// <summary>
    /// 验证码令牌（用于后续校验）
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Base64 图片（data:image/png;base64,...）
    /// </summary>
    public string ImageBase64 { get; set; }

    /// <summary>
    /// 过期秒数
    /// </summary>
    public int ExpiresInSeconds { get; set; }
}


