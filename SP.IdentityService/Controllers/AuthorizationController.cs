using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using SP.Common;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Redis;
using SP.IdentityService.Models.Request;
using SP.IdentityService.Service;

namespace SP.IdentityService.Controllers;

/// <summary>
/// 授权控制器
/// </summary>
[Route("api/auth")]
[ApiController]
public class AuthorizationController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<AuthorizationController> _logger;
    private readonly IRedisService _redisService;
    private readonly ContextSession _contextSession;

    /// <summary>
    /// 授权控制器构造函数
    /// </summary>
    /// <param name="authorizationService"></param>
    /// <param name="logger"></param>
    /// <param name="redisService"></param>
    /// <param name="contextSession"></param>
    public AuthorizationController(IAuthorizationService authorizationService,
        ILogger<AuthorizationController> logger, IRedisService redisService,ContextSession contextSession)
    {
        _logger = logger;
        _authorizationService = authorizationService;
        _redisService = redisService;
        _contextSession = contextSession;
    }

    /// <summary>
    /// 获取访问令牌
    /// </summary>
    /// <remarks>
    /// 请求示例:
    /// 
    ///     POST /api/auth/token
    ///     Content-Type: application/x-www-form-urlencoded
    ///     
    ///     grant_type=password&amp;username=admin&amp;password=123*asdasd&amp;scope=api offline_access
    ///
    ///     或者刷新令牌:
    ///     
    ///     grant_type=refresh_token&amp;refresh_token=YOUR_REFRESH_TOKEN&amp;scope=api
    ///     
    ///     或者客户端凭证模式:
    ///     
    ///     grant_type=client_credentials&amp;client_id=YOUR_CLIENT_ID&amp;client_secret=YOUR_CLIENT_SECRET&amp;scope=api
    ///
    /// 注意：
    /// 1. 必须使用表单（form-data）方式提交，Content-Type为application/x-www-form-urlencoded
    /// 2. 不要将参数放在URL查询字符串中
    /// 3. 在刷新令牌模式下，refresh_token必须放在请求体中，不能放在URL中
    /// 4. 客户端凭证模式适用于服务器到服务器的API调用，不关联特定用户
    /// </remarks>
    /// <returns>返回访问令牌信息</returns>
    /// <response code="200">返回访问令牌</response>
    /// <response code="400">请求格式不正确或不支持的授权类型</response>
    /// <response code="403">认证失败</response>
    [HttpPost("token")]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> GetToken()
    {
        // 检查是否通过查询参数传递敏感信息
        if (Request.Query.Count > 0 &&
            (Request.Query.ContainsKey("refresh_token") || Request.Query.ContainsKey("password") ||
             Request.Query.ContainsKey("client_secret")))
        {
            throw new BadRequestException("不要在URL中包含敏感信息，请使用表单提交方式");
        }

        // 检查请求头中是否包含敏感信息
        if (Request.Headers.Any(h => h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase) &&
                                     h.Value.ToString().Contains("Basic")))
        {
            // 记录警告日志，但允许继续处理，因为某些客户端可能使用Basic认证
            _logger.LogWarning("检测到使用Basic认证，建议改用表单提交方式");
        }

        var request = HttpContext.GetOpenIddictServerRequest();
        if (request == null)
        {
            throw new BadRequestException("请求格式不正确，请使用表单(application/x-www-form-urlencoded)提交");
        }

        // 处理资源所有者密码模式
        if (request.IsPasswordGrantType())
        {
            // 验证用户名和密码
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                throw new BusinessException("用户名或密码不能为空");
            }

            var principal =
                await _authorizationService.LoginByPasswordAsync(request.Username, request.Password,
                    request.GetScopes());
            // 确保 SignIn 方法只在授权端点调用
            SignInResult signInResult = SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            
            // 注意：在 OpenIddict 中，token 是在中间件处理过程中生成的
            // SignInResult 的 Properties 可能不会立即包含 token 值
            // 我们将在响应处理完成后存储 token
            
            return signInResult;
        }

        // 处理刷新令牌
        if (request.IsRefreshTokenGrantType())
        {
            // 从 request 上下文中获取之前存储的身份验证票据
            var principal =
                (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme))
                ?.Principal;

            var newPrincipal =
                await _authorizationService.RefreshTokenAsync(request.RefreshToken, request.GetScopes(), principal);

            var signInResult = SignIn(newPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            
            // 注意：token 将在 OpenIddict 中间件处理过程中生成
            return signInResult;
        }

        // 处理客户端凭证模式
        if (request.IsClientCredentialsGrantType())
        {
            var clientId = request.ClientId;
            if (string.IsNullOrEmpty(clientId))
            {
                throw new BusinessException("client_id不能为空");
            }

            var principal =
                await _authorizationService.HandleClientCredentialsAsync(clientId, request.GetScopes());
            
            var signInResult = SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            
            // 注意：token 将在 OpenIddict 中间件处理过程中生成
            return signInResult;
        }

        // 不支持的授权类型
        return BadRequest(new
        {
            error = OpenIddictConstants.Errors.UnsupportedGrantType,
            error_description = "不支持的授权类型。"
        });
    }

    /// <summary>
    /// 注册用户
    /// </summary>
    /// <param name="user"></param>
    [HttpPost("register")]
    public async Task<ActionResult<long>> Register([FromBody] UserAddRequest user)
    {
        var result = await _authorizationService.AddUserAsync(user);
        return Ok(result);
    }

    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="email"></param>
    [HttpPost("emails")]
    public async Task<ActionResult> SendEmail([FromBody] SendEmailRequest email)
    {
        await _authorizationService.SendEmailAsync(email);
        return Ok();
    }

    /// <summary>
    /// 绑定邮箱
    /// </summary>
    /// <param name="verifyCode"></param>
    [HttpPost("email/bind")]
    public async Task<ActionResult> BindEmail([FromBody] VerifyCodeRequest verifyCode)
    {
        await _authorizationService.AddEmailAsync(verifyCode);
        return Ok();
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="resetPasswordRequest"></param>
    [HttpPut("password/reset")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
    {
        await _authorizationService.ResetPasswordAsync(resetPasswordRequest);
        return Ok();
    }
    
    /// <summary>
    /// 退出登录
    /// </summary>
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        try
        {
            // 获取当前用户ID
            var userId = _contextSession.UserId;
            var username = _contextSession.UserName;
            
            // 1. 清除 Redis 中的 token
            string tokenKey = string.Format(SPRedisKey.Token, userId);
            await _redisService.RemoveAsync(tokenKey);
                
            // 2. 清除相关的刷新令牌（如果有的话）
            string refreshTokenKey = string.Format("RefreshToken:{0}", userId);
            await _redisService.RemoveAsync(refreshTokenKey);
                
            // 3. 记录登出日志
            _logger.LogInformation("用户 {Username} (ID: {UserId}) 已退出登录", username, userId);
                
            return Ok(new { 
                message = "已成功退出登录并撤销令牌",
                user_id = userId,
                username = username
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "退出登录时发生错误");
            return StatusCode(500, new { 
                error = "InternalServerError",
                error_description = "退出登录时发生内部错误"
            });
        }
    }

    /// <summary>
    /// OpenIddict 退出端点
    /// </summary>
    /// <remarks>
    /// 符合 OpenID Connect 规范的退出端点
    /// </remarks>
    [HttpPost("connect/logout")]
    public async Task<ActionResult> OpenIddictLogout()
    {
        try
        {
            // 获取 OpenIddict 请求
            var request = HttpContext.GetOpenIddictServerRequest();
            if (request == null)
            {
                return BadRequest(new
                {
                    error = OpenIddictConstants.Errors.InvalidRequest,
                    error_description = "无效的退出请求"
                });
            }

            // 获取当前用户信息
            var userId = _contextSession.UserId;
            var username = _contextSession.UserName;
            
            // 1. 清除访问令牌
            string tokenKey = string.Format(SPRedisKey.Token, userId);
            await _redisService.RemoveAsync(tokenKey);
                
            // 2. 清除刷新令牌
            string refreshTokenKey = string.Format("RefreshToken:{0}", userId);
            await _redisService.RemoveAsync(refreshTokenKey);
                
            // 3. 记录退出日志
            _logger.LogInformation("用户 {Username} (ID: {UserId}) 通过 OpenIddict 退出", username, userId);

            // 使用 OpenIddict 的标准退出方法
            return SignOut(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenIddict 退出时发生错误");
            return StatusCode(500, new { 
                error = "InternalServerError",
                error_description = "退出时发生内部错误"
            });
        }
    }

    /// <summary>
    /// 撤销令牌
    /// </summary>
    /// <remarks>
    /// 用于撤销访问令牌和刷新令牌，符合 OpenID Connect 规范
    /// </remarks>
    [HttpPost("revoke")]
    public async Task<ActionResult> RevokeToken()
    {
        try
        {
            var request = HttpContext.GetOpenIddictServerRequest();
            if (request == null)
            {
                return BadRequest(new
                {
                    error = OpenIddictConstants.Errors.InvalidRequest,
                    error_description = "无效的撤销请求"
                });
            }

            // 获取当前用户信息
            var userId = _contextSession.UserId;
            var username = _contextSession.UserName;
            
            // 1. 清除访问令牌
            string tokenKey = string.Format(SPRedisKey.Token, userId);
            await _redisService.RemoveAsync(tokenKey);
                
            // 2. 清除刷新令牌
            string refreshTokenKey = string.Format("RefreshToken:{0}", userId);
            await _redisService.RemoveAsync(refreshTokenKey);
                
            // 3. 如果请求中包含特定的刷新令牌，也清除它
            if (!string.IsNullOrEmpty(request.RefreshToken))
            {
                string specificRefreshTokenKey = string.Format("RefreshToken:{0}:{1}", userId, request.RefreshToken);
                await _redisService.RemoveAsync(specificRefreshTokenKey);
            }
                
            // 4. 记录撤销日志
            _logger.LogInformation("用户 {Username} (ID: {UserId}) 的令牌已被撤销", username, userId);
                
            return Ok(new { 
                message = "令牌已成功撤销",
                user_id = userId,
                username = username,
                revoked_at = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "撤销令牌时发生错误");
            return StatusCode(500, new { 
                error = "InternalServerError",
                error_description = "撤销令牌时发生内部错误"
            });
        }
    }

    /// <summary>
    /// 用户信息端点
    /// </summary>
    /// <remarks>
    /// 符合 OpenID Connect 规范的用户信息端点
    /// </remarks>
    [HttpGet("userinfo")]
    public async Task<ActionResult> GetUserInfo()
    {
        try
        {
            // 获取当前用户信息
            var userId = User.FindFirstValue("sub");
            var username = User.FindFirstValue("username");
            var email = User.FindFirstValue("email");
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    error = OpenIddictConstants.Errors.InvalidToken,
                    error_description = "无效的访问令牌"
                });
            }

            // 返回用户信息
            return Ok(new
            {
                sub = userId,
                name = username,
                email = email,
                updated_at = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户信息时发生错误");
            return StatusCode(500, new { 
                error = "InternalServerError",
                error_description = "获取用户信息时发生内部错误"
            });
        }
    }
}