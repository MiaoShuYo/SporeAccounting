using Microsoft.IdentityModel.Tokens;
using SporeAccounting.Server.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SporeAccounting.Middlewares;

/// <summary>
/// 权限中间件
/// </summary>
public class PermissionsMiddleware
{
    private readonly RequestDelegate _next;

    public PermissionsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// 权限中间件
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="sysRoleUrlServer"></param>
    /// <param name="configuration"></param>
    public async System.Threading.Tasks.Task Invoke(HttpContext httpContext, ISysRoleUrlServer sysRoleUrlServer, IConfiguration configuration)
    {
        //请求的路径
        string requestPath = httpContext.Request.Path.Value;
        //如果是登录、注册、找回密码接口，直接放行
        if (requestPath.Contains("/api/SysUser/Login") ||
           requestPath.Contains("/api/SysUser/Register") ||
           requestPath.Contains("/api/SysUser/RetrievePassword"))
        {
            await _next(httpContext);
            return;
        }
        //解析token
        string token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (token == null)
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            await httpContext.Response.WriteAsync("权限不足：用户无权访问此资源。");
        }
        else
        {
            // 验证并解析 Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["JWT:IssuerSigningKey"]);
            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    ValidAudience = configuration["JWT:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true
                }, out SecurityToken validatedToken);
                // 访问 Claims
                var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleId = claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value;
                // 在上下文中存储用户信息
                httpContext.Request.Headers.Add("UserId",userId);
                string pathUrlNotParam = string.Join("/", requestPath);
                bool isUse = sysRoleUrlServer.IsRoleUseUrl(roleId, pathUrlNotParam);
                if (!isUse)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await httpContext.Response.WriteAsync("权限不足：用户无权访问此资源。");
                    return;
                }
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // Token 无效
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsync("无效token");
                return;
            }
        }
    }
}