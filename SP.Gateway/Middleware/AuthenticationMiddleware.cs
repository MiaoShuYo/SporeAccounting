using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
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
        
        var requiresAuthentication = await _configService.IsAuthenticationRequiredAsync(path);
        
        if (!requiresAuthentication)
        {
            _logger.LogDebug("路径 {Path} 跳过认证", path);
            await _next(context);
            return;
        }

        try
        {
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

            context.Request.Headers["X-Identity-Service-Url"] = bestUrl;

            var result = await context.AuthenticateAsync();
            
            if (result?.Succeeded == true && result.Principal != null)
            {
                var userId = result.Principal.FindFirstValue(OpenIddictConstants.Claims.Subject);
                if (!string.IsNullOrEmpty(userId))
                {
                    var tokenExists = await ValidateTokenInRedis(userId, context.Request.Headers["Authorization"].ToString());
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

                context.User = result.Principal;
                
                var userInfo = ExtractUserInfo(result.Principal);
                foreach (var kvp in userInfo)
                {
                    context.Request.Headers[kvp.Key] = kvp.Value;
                }
                
                context.Request.Headers["X-Used-Identity-Service"] = bestUrl;
                
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { 
                    error = "invalid_token", 
                    error_description = "无效的访问令牌" 
                });
            }
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

    private async Task<bool> ValidateTokenInRedis(string userId, string authorizationHeader)
    {
        try
        {
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return false;
            }

            var token = authorizationHeader.Substring("Bearer ".Length);
            
            var tokenKey = $"Token:{userId}";
            var storedToken = await _redisService.GetAsync<string>(tokenKey);
            
            if (string.IsNullOrEmpty(storedToken))
            {
                return true;
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