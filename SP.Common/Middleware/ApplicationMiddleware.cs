using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SP.Common.ExceptionHandling.Exceptions;

namespace SP.Common.Middleware;

/// <summary>
/// 应用程序中间件，所有微服务都要引入
/// </summary>
public class ApplicationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApplicationMiddleware> _logger;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// 应用程序中间件构造函数
    /// </summary>
    /// <param name="next">下一个中间件</param>
    /// <param name="logger">日志记录器</param>
    /// <param name="configuration">配置</param>
    public ApplicationMiddleware(RequestDelegate next, ILogger<ApplicationMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// 中间件处理请求
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <returns>异步任务</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // 验证网关签名
        if (!ValidateGatewaySignature(context))
        {
            _logger.LogWarning("检测到未授权的直接访问，IP: {IP}", context.Connection.RemoteIpAddress);
            throw new UnauthorizedException("未授权的访问");
        }

        if (context.Request.Headers.ContainsKey("X-Anonymous"))
        {
            await _next(context);
            return;
        }
        // 从header中获取用户信息
        var userId = context.Request.Headers["X-User-Id"].FirstOrDefault();
        var username= context.Request.Headers["X-User-Name"].FirstOrDefault();
        var email = context.Request.Headers["X-User-Email"].FirstOrDefault();
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
        {
            _logger.LogError("请求头中缺少用户信息");
            throw new UnauthorizedException("未登录");
        }

        // 创建 ClaimsIdentity 并添加 claims
        var claims = new List<Claim>();
        if (!string.IsNullOrEmpty(userId))
            claims.Add(new Claim("UserId", userId));
        if (!string.IsNullOrEmpty(username))
            claims.Add(new Claim("UserName", username));
        if (!string.IsNullOrEmpty(email))
            claims.Add(new Claim("Email", email));

        var identity = new ClaimsIdentity(claims, "header");
        context.User = new ClaimsPrincipal(identity);

        await _next(context);
    }

    private bool ValidateGatewaySignature(HttpContext context)
    {
        try
        {
            var signature = context.Request.Headers["X-Gateway-Signature"].FirstOrDefault();
            if (string.IsNullOrEmpty(signature))
            {
                return false;
            }

            var signatureBytes = Convert.FromBase64String(signature);
            var signatureText = System.Text.Encoding.UTF8.GetString(signatureBytes);
            var parts = signatureText.Split('.');
            
            if (parts.Length != 2)
            {
                return false;
            }

            if (!long.TryParse(parts[0], out var timestamp))
            {
                return false;
            }

            var secret = _configuration["GatewaySecret"] ?? "SP_Gateway_Secret_2024";
            if (parts[1] != secret)
            {
                return false;
            }

            // 验证时间戳（5分钟内的请求有效）
            var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var timeDiff = Math.Abs(currentTimestamp - timestamp);
            if (timeDiff > 300) // 5分钟
            {
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证网关签名时发生错误");
            return false;
        }
    }
}