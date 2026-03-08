using System.ComponentModel.DataAnnotations;

namespace SP.IdentityService.Models.Request;

/// <summary>
/// 校验验证码请求
/// </summary>
public class CaptchaVerifyRequest
{
    /// <summary>
    /// 验证码令牌
    /// </summary>
    [Required]
    public string Token { get; set; }

    /// <summary>
    /// 用户输入验证码
    /// </summary>
    [Required]
    public string Code { get; set; }
}


