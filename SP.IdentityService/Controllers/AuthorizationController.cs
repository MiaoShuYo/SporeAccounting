using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace SP.IdentityService.Controllers;

/// <summary>
/// 授权控制器
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthorizationController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthorizationController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    /// <summary>
    /// 令牌端点
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [HttpPost("token")]
    public async Task<IActionResult> Token()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
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
            var user = await _userManager.FindByNameAsync(request.Username);
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

            // 返回 SignIn 结果，OpenIddict 将生成访问令牌
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        // 处理客户端凭证模式
        if (request.IsClientCredentialsGrantType())
        {
            // 客户端凭证模式下，客户端身份已由 OpenIddict 验证器验证，直接使用 client_id 作为主题标识
            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            identity.AddClaim(OpenIddictConstants.Claims.Subject,
                request.ClientId ?? throw new InvalidOperationException());
            identity.AddClaim(OpenIddictConstants.Claims.Audience, "api"); // 添加 aud 声明
            var principal = new ClaimsPrincipal(identity);
            principal.SetScopes(request.GetScopes());
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