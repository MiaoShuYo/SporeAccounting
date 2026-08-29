namespace SP.IdentityService.Models.Response;

/// <summary>
/// 令牌内省结果
/// </summary>
/// <summary>
/// 令牌内省结果模型
/// </summary>
public class TokenIntrospectionResponse
{
    /// <summary>
    /// 令牌是否有效
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// 用户唯一标识（sub）
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// 用户邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 令牌作用域
    /// </summary>
    public string? Scope { get; set; }

    /// <summary>
    /// 客户端ID
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// 令牌类型
    /// </summary>
    public string? TokenType { get; set; }

    /// <summary>
    /// 签发时间（Unix时间戳）
    /// </summary>
    public long? IssuedAt { get; set; }

    /// <summary>
    /// 过期时间（Unix时间戳）
    /// </summary>
    public long? ExpiresAt { get; set; }

    /// <summary>
    /// 生效时间（Unix时间戳）
    /// </summary>
    public long? NotBefore { get; set; }

    /// <summary>
    /// 受众
    /// </summary>
    public string? Audience { get; set; }

    /// <summary>
    /// 签发者
    /// </summary>
    public string? Issuer { get; set; }

    /// <summary>
    /// JWT唯一标识
    /// </summary>
    public string? JwtId { get; set; }

    /// <summary>
    /// 角色列表
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// 权限列表
    /// </summary>
    public List<string> Permissions { get; set; } = new();
}