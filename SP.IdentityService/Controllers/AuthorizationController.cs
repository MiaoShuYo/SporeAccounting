using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
    private readonly IOpenIddictApplicationManager _applicationManager;

    public AuthorizationController(
        UserManager<SpUser> userManager,
        SignInManager<SpUser> signInManager,
        IOpenIddictApplicationManager applicationManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _applicationManager = applicationManager;
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
    public async Task<IActionResult> Token()
    {
        // 检查是否通过查询参数传递敏感信息
        if (Request.Query.Count > 0 && (Request.Query.ContainsKey("refresh_token") || Request.Query.ContainsKey("password")))
        {
            return BadRequest(new
            {
                error = "invalid_request",
                error_description = "不要在URL中包含敏感信息(refresh_token/password)，请使用表单提交方式"
            });
        }
        
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request == null)
        {
            return BadRequest(new 
            { 
                error = "invalid_request",
                error_description = "请求格式不正确，请使用表单(application/x-www-form-urlencoded)提交" 
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
            
            // 正确设置范围
            var requestedScopes = request.GetScopes();
            if (requestedScopes.Any())
            {
                // 验证范围是否有效
                var validScopes = new[] { "api", OpenIddictConstants.Scopes.OfflineAccess };
                var filteredScopes = requestedScopes.Intersect(validScopes).ToList();
                
                if (filteredScopes.Any())
                {
                    principal.SetScopes(filteredScopes);
                }
                else
                {
                    // 如果没有有效范围，默认设置为 api
                    principal.SetScopes("api");
                }
            }
            else
            {
                // 默认设置为 api
                principal.SetScopes("api");
            }
            
            // 设置资源
            principal.SetResources("api");
            
            // 允许刷新令牌
            principal.SetRefreshTokenLifetime(TimeSpan.FromDays(14));
            principal.SetAccessTokenLifetime(TimeSpan.FromMinutes(30));

            // 确保 SignIn 方法只在授权端点调用
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        
        // 处理刷新令牌
        if (request.IsRefreshTokenGrantType())
        {
            // 从 request 上下文中获取之前存储的身份验证票据
            var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme))?.Principal;
            
            if (principal == null)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "提供的刷新令牌无效或已过期。"
                    }));
            }
            
            // 检索用户身份
            var userId = principal.GetClaim(OpenIddictConstants.Claims.Subject);
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "用户不存在。"
                    }));
            }

            // 如果用户被禁用或锁定，返回错误
            if (!await _userManager.IsEmailConfirmedAsync(user) || await _userManager.IsLockedOutAsync(user))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "用户被禁用或锁定。"
                    }));
            }

            // 创建新的ClaimsPrincipal
            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            identity.AddClaim(OpenIddictConstants.Claims.Subject, await _userManager.GetUserIdAsync(user));
            identity.AddClaim(OpenIddictConstants.Claims.Name, user.UserName);
            identity.AddClaim(OpenIddictConstants.Claims.Email, user.Email);
            identity.AddClaim(OpenIddictConstants.Claims.Audience, "api");

            // 添加角色声明
            foreach (var role in await _userManager.GetRolesAsync(user))
            {
                identity.AddClaim(ClaimTypes.Role, role);
            }

            var newPrincipal = new ClaimsPrincipal(identity);
            
            // 正确设置范围
            var requestedScopes = request.GetScopes();
            if (requestedScopes.Any())
            {
                // 验证范围是否有效
                var validScopes = new[] { "api", OpenIddictConstants.Scopes.OfflineAccess };
                var filteredScopes = requestedScopes.Intersect(validScopes).ToList();
                
                if (filteredScopes.Any())
                {
                    newPrincipal.SetScopes(filteredScopes);
                }
                else
                {
                    // 如果没有有效范围，默认设置为 api
                    newPrincipal.SetScopes("api");
                }
            }
            else
            {
                // 默认设置为 api
                newPrincipal.SetScopes("api");
                
                // 如果原始令牌有离线访问范围，保留它
                if (principal.HasScope(OpenIddictConstants.Scopes.OfflineAccess))
                {
                    newPrincipal.SetScopes(OpenIddictConstants.Scopes.OfflineAccess);
                }
            }
            
            // 设置资源
            newPrincipal.SetResources("api");
            
            // 设置令牌生命周期
            newPrincipal.SetRefreshTokenLifetime(TimeSpan.FromDays(14));
            newPrincipal.SetAccessTokenLifetime(TimeSpan.FromMinutes(30));

            return SignIn(newPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        // 处理客户端凭证模式
        if (request.IsClientCredentialsGrantType())
        {
            var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                              throw new InvalidOperationException("The application cannot be found.");

            // 创建一个新的ClaimsIdentity，包含将用于创建id_token、token或code的声明。
            var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, OpenIddictConstants.Claims.Name, OpenIddictConstants.Claims.Role);

            // 使用client_id作为主体标识符。
            identity.SetClaim(OpenIddictConstants.Claims.Subject, await _applicationManager.GetClientIdAsync(application));
            identity.SetClaim(OpenIddictConstants.Claims.Name, await _applicationManager.GetDisplayNameAsync(application));
            
            // 添加受众声明
            identity.SetClaim(OpenIddictConstants.Claims.Audience, "api");

            // 设置声明的目标
            identity.SetDestinations(static claim => claim.Type switch
            {
                // 当授予"profile"范围时（通过调用principal.SetScopes(...)），
                // 允许"name"声明同时存储在访问令牌和身份令牌中。
                OpenIddictConstants.Claims.Name when claim.Subject.HasScope(OpenIddictConstants.Permissions.Scopes.Profile)
                    => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],

                // 否则，仅将声明存储在访问令牌中。
                _ => [OpenIddictConstants.Destinations.AccessToken]
            });
            
            var principal = new ClaimsPrincipal(identity);
            
            // 正确设置范围
            var requestedScopes = request.GetScopes();
            if (requestedScopes.Any())
            {
                // 验证范围是否有效
                var validScopes = new[] { "api" };
                var filteredScopes = requestedScopes.Intersect(validScopes).ToList();
                
                if (filteredScopes.Any())
                {
                    principal.SetScopes(filteredScopes);
                }
                else
                {
                    // 如果没有有效范围，默认设置为 api
                    principal.SetScopes("api");
                }
            }
            else
            {
                // 默认设置为 api
                principal.SetScopes("api");
            }
            
            // 设置资源
            principal.SetResources("api");
            
            // 设置令牌生命周期
            principal.SetAccessTokenLifetime(TimeSpan.FromHours(1)); // 客户端凭证默认1小时有效期

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
