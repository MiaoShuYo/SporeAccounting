namespace SP.IdentityService.Models.Request;

/// <summary>
/// 请求验证邮箱
/// </summary>
public class VerifyEmailRequest
{
    /// <summary>
    /// 验证码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 邮箱地址
    /// </summary>
    public string Email { get; set; }
}