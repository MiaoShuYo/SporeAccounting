namespace SP.IdentityService.Services;

/// <summary>
/// 客户端注册服务接口
/// </summary>
public interface IClientRegistrationService
{
    /// <summary>
    /// 注册客户端
    /// </summary>
    /// <param name="clientId">客户端ID</param>
    /// <param name="clientSecret">客户端密钥</param>
    /// <param name="displayName">显示名称</param>
    /// <param name="permissions">权限</param>
    /// <returns>是否成功</returns>
    Task<bool> RegisterClientAsync(string clientId, string clientSecret, string displayName, string[] permissions);
    
    /// <summary>
    /// 删除客户端
    /// </summary>
    /// <param name="clientId">客户端ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteClientAsync(string clientId);
    
    /// <summary>
    /// 获取客户端列表
    /// </summary>
    /// <returns>客户端列表</returns>
    Task<List<ClientInfo>> GetClientsAsync();
    
    /// <summary>
    /// 验证客户端是否存在
    /// </summary>
    /// <param name="clientId">客户端ID</param>
    /// <returns>是否存在</returns>
    Task<bool> ClientExistsAsync(string clientId);
    
    /// <summary>
    /// 初始化默认客户端（用于测试）
    /// </summary>
    /// <returns>是否成功</returns>
    Task<bool> InitializeDefaultClientsAsync();
}

/// <summary>
/// 客户端信息
/// </summary>
public class ClientInfo
{
    /// <summary>
    /// 客户端ID
    /// </summary>
    public string ClientId { get; set; } = string.Empty;
    
    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>
    /// 客户端类型
    /// </summary>
    public string ClientType { get; set; } = string.Empty;
    
    /// <summary>
    /// 权限列表
    /// </summary>
    public List<string> Permissions { get; set; } = new();
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
