namespace SP.Gateway.Models;

/// <summary>
/// 表示令牌内省（Token Introspection）接口的响应结果。
/// </summary>
public class TokenIntrospectionResponse
{
    /// <summary>
    /// 令牌是否处于激活状态。
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 令牌关联的主体（Subject）。
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// 令牌关联的用户名。
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// 令牌关联的邮箱地址。
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 令牌的作用域（Scope）。
    /// </summary>
    public string? Scope { get; set; }

    /// <summary>
    /// 客户端标识（ClientId）。
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// 令牌过期时间（Unix时间戳，秒）。
    /// </summary>
    public long? ExpiresAt { get; set; }

    /// <summary>
    /// 令牌关联的角色列表。
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// 令牌关联的权限列表。
    /// </summary>
    public List<string> Permissions { get; set; } = new();
}