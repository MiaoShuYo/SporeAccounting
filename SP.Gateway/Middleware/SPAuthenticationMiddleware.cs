using System.Security.Claims;
using OpenIddict.Abstractions;
using SP.Common.Redis;
using SP.Gateway.Services;


namespace SP.Gateway.Middleware;

/// <summary>
/// 完整的认证中间件
/// </summary>
public class SPAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SPAuthenticationMiddleware> _logger;
    private readonly IRedisService _redisService;
    private readonly IGatewayConfigService _configService;
    private readonly INacosServiceDiscoveryService _serviceDiscovery;
    private readonly ITokenIntrospectionService _tokenIntrospectionService;

    public SPAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<SPAuthenticationMiddleware> logger,
        IRedisService redisService,
        IGatewayConfigService configService,
        INacosServiceDiscoveryService serviceDiscovery,
        ITokenIntrospectionService tokenIntrospectionService)
    {
        _next = next;
        _logger = logger;
        _redisService = redisService;
        _configService = configService;
        _serviceDiscovery = serviceDiscovery;
        _tokenIntrospectionService = tokenIntrospectionService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "";
        
        var isAuth = await _configService.IsAuthenticationRequiredAsync(path);
        
        if (!isAuth)
        {
            _logger.LogDebug("路径 {Path} 跳过认证", path);
            await _next(context);
            return;
        }

        try
        {
            // 获取Authorization头
            var authorizationHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { 
                    error = "invalid_token", 
                    error_description = "缺少有效的访问令牌" 
                });
                return;
            }

            var token = authorizationHeader.Substring("Bearer ".Length);
            
            // 获取身份服务URL
            var bestUrl = await _serviceDiscovery.GetBestIdentityServiceUrlAsync();
            if (string.IsNullOrEmpty(bestUrl))
            {
                context.Response.StatusCode = 503;
                await context.Response.WriteAsJsonAsync(new { 
                    error = "service_unavailable", 
                    error_description = "身份服务不可用" 
                });
                return;
            }

            // 使用令牌内省服务验证令牌
            var introspectionResult = await _tokenIntrospectionService.IntrospectTokenAsync(token, bestUrl);
            if (introspectionResult == null || !introspectionResult.IsActive)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { 
                    error = "invalid_token", 
                    error_description = "无效的访问令牌" 
                });
                return;
            }

            // 验证Redis中的令牌
            if (!string.IsNullOrEmpty(introspectionResult.Subject))
            {
                var tokenExists = await ValidateTokenInRedis(introspectionResult.Subject, token);
                if (!tokenExists)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { 
                        error = "invalid_token", 
                        error_description = "令牌已被撤销" 
                    });
                    return;
                }
            }

            // 创建ClaimsPrincipal
            var claims = new List<Claim>
            {
                new Claim(OpenIddictConstants.Claims.Subject, introspectionResult.Subject ?? ""),
                new Claim(OpenIddictConstants.Claims.Name, introspectionResult.Username ?? ""),
                new Claim(OpenIddictConstants.Claims.Email, introspectionResult.Email ?? "")
            };

            // 添加角色声明
            if (introspectionResult.Roles != null && introspectionResult.Roles.Any())
            {
                foreach (var role in introspectionResult.Roles)
                {
                    claims.Add(new Claim(OpenIddictConstants.Claims.Role, role));
                }
            }

            var identity = new ClaimsIdentity(claims, "Bearer");
            var principal = new ClaimsPrincipal(identity);
            
            context.User = principal;
            
            // 添加用户信息到请求头
            var userInfo = ExtractUserInfo(principal);
            foreach (var kvp in userInfo)
            {
                context.Request.Headers[kvp.Key] = kvp.Value;
            }
            
            context.Request.Headers["X-Used-Identity-Service"] = bestUrl;
            
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "认证过程中发生错误");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { 
                error = "server_error", 
                error_description = "认证服务内部错误" 
            });
        }
    }

    private async Task<bool> ValidateTokenInRedis(string userId, string token)
    {
        try
        {
            var tokenKey = $"Token:{userId}";
            var storedToken = await _redisService.GetAsync<string>(tokenKey);
            
            if (string.IsNullOrEmpty(storedToken))
            {
                return true; // 如果Redis中没有存储令牌，认为有效
            }
            
            return storedToken == token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证Redis中的令牌时发生错误");
            return false;
        }
    }

    private Dictionary<string, string> ExtractUserInfo(ClaimsPrincipal principal)
    {
        var userInfo = new Dictionary<string, string>();
        
        var userId = principal.FindFirstValue(OpenIddictConstants.Claims.Subject);
        var username = principal.FindFirstValue(OpenIddictConstants.Claims.Name);
        var email = principal.FindFirstValue(OpenIddictConstants.Claims.Email);
        var roles = principal.FindAll(OpenIddictConstants.Claims.Role).Select(c => c.Value);
        
        if (!string.IsNullOrEmpty(userId))
            userInfo["X-User-Id"] = userId;
        if (!string.IsNullOrEmpty(username))
            userInfo["X-User-Name"] = username;
        if (!string.IsNullOrEmpty(email))
            userInfo["X-User-Email"] = email;
        if (roles.Any())
            userInfo["X-User-Roles"] = string.Join(",", roles);
            
        return userInfo;
    }
}