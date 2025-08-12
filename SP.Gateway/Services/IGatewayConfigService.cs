using SP.Gateway.Models;

namespace SP.Gateway.Services;

/// <summary>
/// 网关配置服务接口
/// </summary>
public interface IGatewayConfigService
{
    /// <summary>
    /// 获取跳过认证的路径
    /// </summary>
    /// <returns>跳过认证的路径列表</returns>
    Task<List<string>> GetSkipAuthenticationPathsAsync();
    
    /// <summary>
    /// 获取需要认证的路径
    /// </summary>
    /// <returns>需要认证的路径列表</returns>
    Task<List<string>> GetRequireAuthenticationPathsAsync();
    
    /// <summary>
    /// 检查路径是否需要认证
    /// </summary>
    /// <param name="path">请求路径</param>
    /// <returns>是否需要认证</returns>
    Task<bool> IsAuthenticationRequiredAsync(string path);
    
    /// <summary>
    /// 获取身份服务配置
    /// </summary>
    /// <returns>身份服务配置</returns>
    Task<IdentityServiceConfig> GetIdentityServiceConfigAsync();
}