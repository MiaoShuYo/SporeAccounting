using SP.IdentityService.Models.Response;

namespace SP.IdentityService.Services;

/// <summary>
/// 图形验证码服务
/// </summary>
public interface ICaptchaService
{
    /// <summary>
    /// 生成图形验证码（字母数字）并缓存到 Redis
    /// </summary>
    /// <param name="ip">请求方IP（可选用于限流）</param>
    /// <returns>验证码图片与令牌</returns>
    Task<CaptchaCreateResponse> CreateAsync(string? ip = null);

    /// <summary>
    /// 校验图形验证码
    /// </summary>
    /// <param name="token">验证码令牌</param>
    /// <param name="code">用户输入验证码</param>
    /// <param name="removeOnSuccess">成功后是否删除</param>
    /// <returns>是否通过</returns>
    Task<bool> VerifyAsync(string token, string code, bool removeOnSuccess = true);
}


