using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using SP.IdentityService.Models;

namespace SP.IdentityService.Controllers;

/// <summary>
/// 授权控制器
/// </summary>
[Route("connect")]
[ApiController]
public class AuthorizationController : ControllerBase
{
    private readonly UserManager<SpUser> _userManager;
    private readonly SignInManager<SpUser> _signInManager;

    public AuthorizationController(
        UserManager<SpUser> userManager,
        SignInManager<SpUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    /// <summary>
    /// 令牌端点 - 获取访问令牌
    /// </summary>
    /// <remarks>
    /// 请求示例:
    /// 
    ///     POST /connect/token
    ///     Content-Type: application/x-www-form-urlencoded
    ///     
    ///     grant_type=password&amp;username=admin&amp;password=123*asdasd&amp;scope=api
    ///
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
    public async Task<IActionResult> Token()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request == null)
        {
            return BadRequest(new 
            { 
                error = "invalid_request",
                error_description = "请求格式不正确" 
            });
        }
        
        // 处理资源所有者密码模式
        if (request.IsPasswordGrantType())
        {
            // 验证用户名和密码
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                            OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "用户名或密码不能为空。"
                    }));
            }

            // 使用ASP.NET Core Identity验证用户
            SpUser? user = null;
            try
            {
                user = await _userManager.FindByNameAsync(request.Username);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (user == null)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                            OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "用户名不存在。"
                    }));
            }

            // 验证密码
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                            OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            result.IsLockedOut ? "账户已锁定。" : "用户名或密码错误。"
                    }));
            }

            // 创建用户身份并添加必要的声明
            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            identity.AddClaim(OpenIddictConstants.Claims.Subject, await _userManager.GetUserIdAsync(user));
            identity.AddClaim(OpenIddictConstants.Claims.Name, user.UserName);
            identity.AddClaim(OpenIddictConstants.Claims.Email, user.Email);
            identity.AddClaim(OpenIddictConstants.Claims.Audience, "api"); // 添加 aud 声明

            // 添加用户角色
            foreach (var role in await _userManager.GetRolesAsync(user))
            {
                identity.AddClaim(ClaimTypes.Role, role);
            }

            // 创建 ClaimsPrincipal，并设置请求的范围
            var principal = new ClaimsPrincipal(identity);
            principal.SetScopes(request.GetScopes());

            // 确保 SignIn 方法只在授权端点调用
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        // 处理客户端凭证模式
        if (request.IsClientCredentialsGrantType())
        {
            // 客户端凭证模式下，客户端身份已由 OpenIddict 验证器验证，直接使用 client_id 作为主题标识
            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            identity.AddClaim(OpenIddictConstants.Claims.Subject,
                request.ClientId ?? throw new InvalidOperationException());
            identity.AddClaim(OpenIddictConstants.Claims.Audience, string.Join(",", request.GetScopes()));
            var principal = new ClaimsPrincipal(identity);
            principal.SetScopes(request.GetScopes());

            // 确保 SignIn 方法只在授权端点调用
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        // 不支持的授权类型
        return BadRequest(new
        {
            error = OpenIddictConstants.Errors.UnsupportedGrantType,
            error_description = "不支持的授权类型。"
        });
    }
}
