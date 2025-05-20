using System.Collections.Immutable;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Exceptions;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using SP.Common;
using SP.Common.ExceptionHandling.Exceptions;
using SP.IdentityService.Models.Entity;
using SP.IdentityService.Models.Request;

namespace SP.IdentityService.Service.Impl;

public class UserServiceImpl : IUserService
{
    /// <summary>
    /// 用户管理器
    /// </summary>
    private readonly UserManager<SpUser> _userManager;

    /// <summary>
    /// 签名管理器
    /// </summary>
    private readonly SignInManager<SpUser> _signInManager;

    /// <summary>
    /// 应用程序管理器
    /// </summary>
    private readonly IOpenIddictApplicationManager _applicationManager;

    public UserServiceImpl(UserManager<SpUser> userManager,
        SignInManager<SpUser> signInManager,
        IOpenIddictApplicationManager applicationManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _applicationManager = applicationManager;
    }

    /// <summary>
    /// 密码登录
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="scopes"></param>
    /// <returns></returns>
    public async Task<ClaimsPrincipal> LoginByPasswordAsync(string userName, string password,
        ImmutableArray<string> scopes)
    {
        // 使用ASP.NET Core Identity验证用户
        SpUser? user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            throw new BusinessException("用户不存在");
        }

        // 验证密码
        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                throw new BusinessException("账户已锁定，请稍后再试。");
            }

            throw new BusinessException("用户名或密码错误。");
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
        if (scopes.Any())
        {
            // 验证范围是否有效
            var validScopes = new[] { "api", OpenIddictConstants.Scopes.OfflineAccess };
            var filteredScopes = scopes.Intersect(validScopes).ToList();
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
        return principal;
    }

    /// <summary>
    /// 刷新token
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="scopes"></param>
    /// <param name="principal"></param>
    /// <returns></returns>
    public async Task<ClaimsPrincipal> RefreshTokenAsync(string? refreshToken, ImmutableArray<string> scopes,
        ClaimsPrincipal? principal)
    {
        if (principal == null)
        {
            throw new BusinessException("提供的刷新令牌无效或已过期");
        }

        // 检索用户身份
        var userId = principal.GetClaim(OpenIddictConstants.Claims.Subject);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new BusinessException("用户不存在");
        }

        // 如果用户被禁用或锁定，返回错误
        if (!await _userManager.IsEmailConfirmedAsync(user) || await _userManager.IsLockedOutAsync(user))
        {
            throw new BusinessException("用户已被禁用或锁定");
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
        if (scopes.Any())
        {
            // 验证范围是否有效
            var validScopes = new[] { "api", OpenIddictConstants.Scopes.OfflineAccess };
            var filteredScopes = scopes.Intersect(validScopes).ToList();

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
        return newPrincipal;
    }

    /// <summary>
    /// 处理客户端凭证模式
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="scopes"></param>
    /// <returns></returns>
    public async Task<ClaimsPrincipal> HandleClientCredentialsAsync(string clientId, ImmutableArray<string> scopes)
    {
        var application = await _applicationManager.FindByClientIdAsync(clientId) ??
                          throw new BuildAbortedException("找不到应用");

        // 创建一个新的ClaimsIdentity，包含将用于创建id_token、token或code的声明。
        var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType,
            OpenIddictConstants.Claims.Name, OpenIddictConstants.Claims.Role);

        // 使用client_id作为主体标识符。
        identity.SetClaim(OpenIddictConstants.Claims.Subject,
            await _applicationManager.GetClientIdAsync(application));
        identity.SetClaim(OpenIddictConstants.Claims.Name,
            await _applicationManager.GetDisplayNameAsync(application));

        // 添加受众声明
        identity.SetClaim(OpenIddictConstants.Claims.Audience, "api");

        // 设置声明的目标
        identity.SetDestinations(static claim => claim.Type switch
        {
            // 当授予"profile"范围时（通过调用principal.SetScopes(...)），
            // 允许"name"声明同时存储在访问令牌和身份令牌中。
            OpenIddictConstants.Claims.Name when claim.Subject.HasScope(OpenIddictConstants.Permissions.Scopes
                    .Profile)
                => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],

            // 否则，仅将声明存储在访问令牌中。
            _ => [OpenIddictConstants.Destinations.AccessToken]
        });

        var principal = new ClaimsPrincipal(identity);

        // 正确设置范围
        if (scopes.Any())
        {
            // 验证范围是否有效
            var validScopes = new[] { "api" };
            var filteredScopes = scopes.Intersect(validScopes).ToList();

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
        return principal;
    }

    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="user"></param>
    /// <returns>用户id</returns>
    public async Task<long> AddUserAsync(UserAddRequest user)
    {
        // 创建用户
        var newUser = new SpUser
        {
            Id = Snow.GetId(),
            UserName = user.UserName,
        };
        var hasher = new PasswordHasher<SpUser>();
        string passwordHash = hasher.HashPassword(newUser, user.Password);
        IdentityResult result = await _userManager.CreateAsync(newUser, passwordHash);
        if (result.Succeeded)
        {
            return newUser.Id;
        }

        throw new Exception(string.Join(",", result.Errors.Select(e => e.Description)));
    }
}