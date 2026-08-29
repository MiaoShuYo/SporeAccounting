using SP.Gateway.Models;

namespace SP.Gateway.Services;

/// <summary>
/// 令牌内省服务接口
/// </summary>
public interface ITokenIntrospectionService
{
    /// <summary>
    /// 验证令牌
    /// </summary>
    /// <param name="token">访问令牌</param>
    /// <param name="identityServiceUrl">身份服务URL</param>
    /// <returns>令牌信息</returns>
    Task<TokenIntrospectionResponse?> IntrospectTokenAsync(string token, string identityServiceUrl);
}