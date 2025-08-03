using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Redis;

namespace SP.Common.Middleware;

/// <summary>
/// 应用程序中间件，所有微服务都要引入
/// </summary>
public class ApplicationMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// 应用程序中间件构造函数
    /// </summary>
    /// <param name="next">下一个中间件</param>
    public ApplicationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// 中间件处理请求
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <param name="redisService">Redis服务</param>
    /// <returns>异步任务</returns>
    public async Task InvokeAsync(HttpContext context,IRedisService redisService)
    {
        // 获取请求路径
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
        // 获取不需要身份验证的路径，从nacos配置里面拿。
        List<string> noAuthPaths = new List<string>();
        noAuthPaths= context.RequestServices.GetService<IConfiguration>()?
            .GetSection("NoAuthPaths")?.Get<List<string>>() ?? new List<string>();
        // 如果请求路径在不需要身份验证的列表中，直接调用下一个中间件
        if (noAuthPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }
        // 获取Authorization头
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var claims = jwtToken.Claims.ToList();
                // 查找UserId和UserName
                var userId = claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                var userName = claims.FirstOrDefault(c => c.Type == "username")?.Value;

                // 检查token是否存在于Redis中
                string tokenKey = string.Format(SPRedisKey.Token, userId);
                string? tokenRedis = await redisService.GetStringAsync(tokenKey);
                if (tokenRedis == null || tokenRedis != token)
                {
                    throw new UnauthorizedException("用户未登录或Token已失效");
                }

                // 如果HttpContext.User没有身份，则新建
                if (context.User == null || !context.User.Identities.Any())
                {
                    var identity = new ClaimsIdentity(claims, "jwt");
                    context.User = new ClaimsPrincipal(identity);
                }
                else
                {
                    // 合并claims到现有identity
                    var identity = context.User.Identities.First();
                    if (!string.IsNullOrEmpty(userId) && !identity.HasClaim(c => c.Type == "UserId"))
                        identity.AddClaim(new Claim("sub", userId));
                    if (!string.IsNullOrEmpty(userName) && !identity.HasClaim(c => c.Type == "UserName"))
                        identity.AddClaim(new Claim("username", userName));
                }

                // 调用下一个中间件
                await _next(context);
            }
            catch
            {
                throw new UnauthorizedException("用户未登录");
            }
        }
        else
        {
            throw new UnauthorizedException("用户未登录");
        }
    }
}