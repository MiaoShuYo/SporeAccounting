namespace SP.Gateway.Models;

/// <summary>
/// 身份服务配置
/// </summary>
public class IdentityServiceConfig
{
    /// <summary>
    /// 客户端ID
    /// </summary>
    public string ClientId { get; set; } = "gateway-client";
    
    /// <summary>
    /// 客户端密钥
    /// </summary>
    public string ClientSecret { get; set; } = "gateway-secret";
    
    /// <summary>
    /// 超时时间（秒）
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
    
    /// <summary>
    /// 重试次数
    /// </summary>
    public int RetryCount { get; set; } = 3;
}