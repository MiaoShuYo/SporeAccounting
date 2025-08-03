using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.ConfigService;

namespace SP.Common.Middleware;

/// <summary>
/// 应用程序中间件，所有微服务都要引入
/// </summary>
public class ApplicationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtConfigService _jwtConfigService;

    /// <summary>
    /// 应用程序中间件构造函数
    /// </summary>
    /// <param name="next">下一个中间件</param>
    /// <param name="jwtConfigService">Jwt配置服务</param>
    public ApplicationMiddleware(RequestDelegate next, JwtConfigService jwtConfigService)
    {
        _next = next;
        _jwtConfigService = jwtConfigService;
    }

    /// <summary>
    /// 中间件处理请求
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <returns>异步任务</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // 1. 获取Authorization头
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