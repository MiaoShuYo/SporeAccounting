using SP.Common.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace SP.IdentityService.Middleware;

/// <summary>
/// Token 存储中间件
/// 用于在 OpenIddict 生成 token 后将其存储到 Redis 中
/// </summary>
public class TokenStorageMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRedisService _redisService;
    private readonly ILogger<TokenStorageMiddleware> _logger;

    public TokenStorageMiddleware(RequestDelegate next, IRedisService redisService, ILogger<TokenStorageMiddleware> logger)
    {
        _next = next;
        _redisService = redisService;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 保存原始的响应流
        var originalBodyStream = context.Response.Body;

        try
        {
            // 创建一个内存流来捕获响应
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            // 继续处理请求
            await _next(context);

            // 检查是否是 token 端点且响应成功
            if (context.Request.Path.StartsWithSegments("/api/auth/token") && 
                context.Response.StatusCode == 200)
            {
                // 重置流位置以读取内容
                memoryStream.Position = 0;
                var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

                try
                {
                    // 解析响应以获取 token
                    var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    
                    if (tokenResponse.TryGetProperty("access_token", out var accessTokenElement))
                    {
                        var accessToken = accessTokenElement.GetString();
                        var expiresIn = 3600; // 默认1小时

                        // 尝试获取过期时间
                        if (tokenResponse.TryGetProperty("expires_in", out var expiresInElement))
                        {
                            expiresIn = expiresInElement.GetInt32();
                        }

                        // 从 JWT token 中解析用户ID
                        string? userId = null;
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            try
                            {
                                var handler = new JwtSecurityTokenHandler();
                                var jwtToken = handler.ReadJwtToken(accessToken);
                                userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "解析 JWT token 失败");
                            }
                        }

                        // 如果无法从 JWT 中获取用户ID，尝试从响应中获取
                        if (string.IsNullOrEmpty(userId) && tokenResponse.TryGetProperty("sub", out var subElement))
                        {
                            userId = subElement.GetString();
                        }

                        // 如果仍然没有用户ID，尝试从请求中获取客户端ID
                        if (string.IsNullOrEmpty(userId))
                        {
                            var clientId = context.Request.Form["client_id"].FirstOrDefault();
                            if (!string.IsNullOrEmpty(clientId))
                            {
                                userId = clientId;
                            }
                        }

                        if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(userId))
                        {
                            // 存储 token 到 Redis
                            string tokenKey = string.Format(SPRedisKey.Token, userId);
                            await _redisService.SetStringAsync(tokenKey, accessToken, expiresIn);
                        }
                        else
                        {
                            _logger.LogWarning("无法获取用户ID或 access_token，跳过 Redis 存储");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "解析 token 响应时发生错误");
                }

                // 将响应内容写回原始流
                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(originalBodyStream);
            }
            else
            {
                // 对于非 token 端点，直接复制响应
                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(originalBodyStream);
            }
        }
        finally
        {
            // 恢复原始响应流
            context.Response.Body = originalBodyStream;
        }
    }
}

/// <summary>
/// Token 存储中间件扩展
/// </summary>
public static class TokenStorageMiddlewareExtensions
{
    public static IApplicationBuilder UseTokenStorage(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenStorageMiddleware>();
    }
} 