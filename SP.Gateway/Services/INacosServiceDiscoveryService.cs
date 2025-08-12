namespace SP.Gateway.Services;

/// <summary>
/// Nacos服务发现服务接口
/// </summary>
public interface INacosServiceDiscoveryService
{
    /// <summary>
    /// 获取身份服务实例列表
    /// </summary>
    /// <returns>身份服务实例列表</returns>
    Task<List<string>> GetIdentityServiceUrlsAsync();
    
    /// <summary>
    /// 获取最佳的身份服务URL
    /// </summary>
    /// <returns>最佳URL</returns>
    Task<string?> GetBestIdentityServiceUrlAsync();
    
    /// <summary>
    /// 检查服务是否可用
    /// </summary>
    /// <param name="url">服务URL</param>
    /// <returns>是否可用</returns>
    Task<bool> IsServiceAvailableAsync(string url);
}