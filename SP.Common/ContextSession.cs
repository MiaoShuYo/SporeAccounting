using Microsoft.AspNetCore.Http;

namespace SP.Common;

/// <summary>
/// 上下文会话（用于保存当前请求的用户信息）
/// </summary>
public class ContextSession
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ContextSession(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// 获取当前请求的用户ID
    /// </summary>
    public long UserId
    {
        get
        {
            // 假设UserId存储在Claims中
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId");
            if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            return 0;
        }
    }

    /// <summary>
    /// 获取当前请求的用户名
    /// </summary>
    public string UserName
    {
        get
        {
            // 假设UserName存储在Claims中
            var userNameClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("UserName");
            return userNameClaim?.Value ?? string.Empty;
        }
    }
}