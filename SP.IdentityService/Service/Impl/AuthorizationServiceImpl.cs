using System.Collections.Immutable;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using SP.Common;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Message.Model;
using SP.Common.Message.Mq;
using SP.Common.Message.Mq.Model;
using SP.Common.Redis;
using SP.IdentityService.DB;
using SP.IdentityService.Models.Entity;
using SP.IdentityService.Models.Request;
using Microsoft.EntityFrameworkCore;
using SP.Common.Message.SmS.Model;
using SP.Common.Message.SmS.Services;
using SP.IdentityService.Models.Enumeration;

namespace SP.IdentityService.Service.Impl;

public class AuthorizationServiceImpl : IAuthorizationService
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

    /// <summary>
    /// RabbitMQ消息服务
    /// </summary>
    private readonly RabbitMqMessage _rabbitMqMessage;

    /// <summary>
    /// Redis服务
    /// </summary>
    private readonly IRedisService _redis;

    private readonly IdentityServerDbContext _dbContext;

    /// <summary>
    /// HTTP上下文访问器
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// 短信服务
    /// </summary>
    private readonly ISmSService _smsService;

    private readonly ILogger<AuthorizationServiceImpl> _log;


    /// <summary>
    /// 用户服务构造函数
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="signInManager"></param>
    /// <param name="applicationManager"></param>
    /// <param name="rabbitMqMessage"></param>
    /// <param name="redis"></param>
    /// <param name="dbContext"></param>
    /// <param name="smsService"></param>
    /// <param name="httpContextAccessor"></param>
    public AuthorizationServiceImpl(UserManager<SpUser> userManager,
        SignInManager<SpUser> signInManager,
        IOpenIddictApplicationManager applicationManager, RabbitMqMessage rabbitMqMessage,
        IRedisService redis, IdentityServerDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ISmSService smsService,
        ILogger<AuthorizationServiceImpl> log)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _applicationManager = applicationManager;
        _rabbitMqMessage = rabbitMqMessage;
        _redis = redis;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _log = log;
        _smsService = smsService;
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
        SpUser? user =
            await _userManager.Users.FirstOrDefaultAsync(u =>
                u.PhoneNumber == userName || u.Email == userName || u.UserName == userName);
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

        // 设置声明的目标 - 指定哪些claims应该包含在访问令牌中
        identity.SetDestinations(static claim => claim.Type switch
        {
            // 当授予"profile"范围时，允许"name"声明同时存储在访问令牌和身份令牌中
            OpenIddictConstants.Claims.Name when claim.Subject.HasScope(OpenIddictConstants.Permissions.Scopes.Profile)
                => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],

            // 当授予"email"范围时，允许"email"声明同时存储在访问令牌和身份令牌中
            OpenIddictConstants.Claims.Email when claim.Subject.HasScope(OpenIddictConstants.Permissions.Scopes.Email)
                => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],

            // 角色声明总是包含在访问令牌中
            ClaimTypes.Role => [OpenIddictConstants.Destinations.AccessToken],

            // 其他声明仅存储在访问令牌中
            _ => [OpenIddictConstants.Destinations.AccessToken]
        });

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

        var roles = await _userManager.GetRolesAsync(user);
        // 根据用户角色或请求来源调整令牌生命周期
        if (roles.Contains("Admin"))
        {
            // 管理员令牌生命周期较短
            principal.SetAccessTokenLifetime(TimeSpan.FromMinutes(15));
            principal.SetRefreshTokenLifetime(TimeSpan.FromDays(7));
        }
        else if (_httpContextAccessor?.HttpContext?.Request.Headers.TryGetValue("User-Agent", out var userAgent) ==
                 true &&
                 userAgent.ToString().Contains("Mobile"))
        {
            // 移动设备令牌生命周期较长
            principal.SetAccessTokenLifetime(TimeSpan.FromHours(1));
            principal.SetRefreshTokenLifetime(TimeSpan.FromDays(30));
        }
        else
        {
            // 默认设置
            principal.SetAccessTokenLifetime(TimeSpan.FromMinutes(30));
            principal.SetRefreshTokenLifetime(TimeSpan.FromDays(14));
        }

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

        // 设置声明的目标 - 指定哪些claims应该包含在访问令牌中
        identity.SetDestinations(static claim => claim.Type switch
        {
            // 当授予"profile"范围时，允许"name"声明同时存储在访问令牌和身份令牌中
            OpenIddictConstants.Claims.Name when claim.Subject.HasScope(OpenIddictConstants.Permissions.Scopes.Profile)
                => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],

            // 当授予"email"范围时，允许"email"声明同时存储在访问令牌和身份令牌中
            OpenIddictConstants.Claims.Email when claim.Subject.HasScope(OpenIddictConstants.Permissions.Scopes.Email)
                => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],

            // 角色声明总是包含在访问令牌中
            ClaimTypes.Role => [OpenIddictConstants.Destinations.AccessToken],

            // 其他声明仅存储在访问令牌中
            _ => [OpenIddictConstants.Destinations.AccessToken]
        });

        var newPrincipal = new ClaimsPrincipal(identity);
        // 在设置新范围之前，保存原始令牌的离线访问范围
        bool hasOfflineAccess = principal.HasScope(OpenIddictConstants.Scopes.OfflineAccess);

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
                var defaultScopes = new List<string> { "api" };

                // 如果原始令牌有离线访问范围，保留它
                if (hasOfflineAccess)
                {
                    defaultScopes.Add(OpenIddictConstants.Scopes.OfflineAccess);
                }

                newPrincipal.SetScopes(defaultScopes);
            }
        }
        else
        {
            // 默认设置为 api
            var defaultScopes = new List<string> { "api" };

            // 如果原始令牌有离线访问范围，保留它
            if (hasOfflineAccess)
            {
                defaultScopes.Add(OpenIddictConstants.Scopes.OfflineAccess);
            }

            newPrincipal.SetScopes(defaultScopes);
        }

        // 设置令牌生命周期
        newPrincipal.SetRefreshTokenLifetime(TimeSpan.FromDays(14));
        newPrincipal.SetAccessTokenLifetime(TimeSpan.FromMinutes(30));
        return newPrincipal;
    }

    /// <summary>
    /// 处理客户端凭证模式
    /// </summary>
    /// <param name="clientId">客户端ID</param>
    /// <param name="clientSecret">客户端密钥</param>
    /// <param name="scopes">授权范围</param>
    /// <returns>ClaimsPrincipal</returns>
    public async Task<ClaimsPrincipal> HandleClientCredentialsAsync(string clientId, string? clientSecret,
        ImmutableArray<string> scopes)
    {
        try
        {
            _log.LogDebug("开始处理客户端凭证模式认证，客户端ID: {ClientId}", clientId);

            // 查找客户端应用
            var application = await _applicationManager.FindByClientIdAsync(clientId);
            if (application == null)
            {
                _log.LogWarning("客户端凭证模式认证失败，客户端ID不存在: {ClientId}", clientId);
                throw new BusinessException($"客户端ID '{clientId}' 不存在");
            }

            // 验证客户端密钥（如果提供了的话）
            if (!string.IsNullOrEmpty(clientSecret))
            {
                // 在 OpenIddict 6.x 中，客户端密钥验证通常由 OpenIddict 内部处理
                // 我们只需要确保客户端存在且类型正确
                var clientType = await _applicationManager.GetClientTypeAsync(application);
                if (clientType != OpenIddictConstants.ClientTypes.Confidential)
                {
                    _log.LogWarning("客户端凭证模式认证失败，客户端类型不支持: {ClientId}, 类型: {ClientType}", clientId, clientType);
                    throw new BusinessException("客户端类型不支持客户端凭证模式");
                }
            }

            // 验证客户端权限
            var permissions = await _applicationManager.GetPermissionsAsync(application);
            if (!permissions.Contains(OpenIddictConstants.Permissions.GrantTypes.ClientCredentials))
            {
                _log.LogWarning("客户端凭证模式认证失败，客户端没有权限: {ClientId}", clientId);
                throw new BusinessException("客户端没有客户端凭证模式的权限");
            }

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

            // 添加客户端ID声明
            identity.SetClaim("client_id", clientId);

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
                    _log.LogDebug("设置客户端范围: {Scopes}", string.Join(", ", filteredScopes));
                }
                else
                {
                    // 如果没有有效范围，默认设置为 api
                    principal.SetScopes("api");
                    _log.LogDebug("使用默认范围: api");
                }
            }
            else
            {
                // 默认设置为 api
                principal.SetScopes("api");
                _log.LogDebug("使用默认范围: api");
            }

            // 设置令牌生命周期（客户端凭证模式通常较短）
            principal.SetAccessTokenLifetime(TimeSpan.FromHours(1));

            _log.LogInformation("客户端凭证模式认证成功，客户端ID: {ClientId}", clientId);
            return principal;
        }
        catch (BusinessException)
        {
            // 重新抛出业务异常
            throw;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "客户端凭证模式认证时发生未预期的错误，客户端ID: {ClientId}", clientId);
            throw new BusinessException("客户端凭证模式认证失败，请稍后重试");
        }
    }

    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="user"></param>
    /// <returns>用户id</returns>
    public async Task<long> AddUserAsync(UserRegisterRequest user)
    {
        long userId = 0;
        switch (user.RegisterType)
        {
            case RegisterTypeEnum.UserName:
                userId = await RegisterByUserNameAsync(user.UserName, user.Password);
                break;
            case RegisterTypeEnum.Email:
                userId = await RegisterByEmailAsync(user.Email, user.Code);
                break;
            case RegisterTypeEnum.PhoneNumber:
                userId = await RegisterByPhoneNumberAsync(user.PhoneNumber, user.Code);
                break;
        }

        // 发送mq，设配默认币种
        MqPublisher publisher = new MqPublisher(userId.ToString(),
            MqExchange.UserConfigExchange,
            MqRoutingKey.UserConfigDefaultCurrencyRoutingKey,
            MqQueue.UserConfigQueue,
            MessageType.UserConfigDefaultCurrency,
            ExchangeType.Direct);
        await _rabbitMqMessage.SendAsync(publisher);
        return userId;
    }

    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns></returns>
    public async Task SendEmailAsync(SendEmailRequest email)
    {
        MqPublisher publisher = new MqPublisher(email.Email,
            MqExchange.EmailExchange,
            MqRoutingKey.EmailRoutingKey,
            MqQueue.EmailQueue,
            email.MessageType,
            ExchangeType.Direct);
        await _rabbitMqMessage.SendAsync(publisher);
    }

    /// <summary>
    /// 添加邮箱
    /// </summary>
    /// <param name="verifyCode"></param>
    /// <exception cref="BusinessException"></exception>
    /// <exception cref="Exception"></exception>
    public async Task AddEmailAsync(VerifyCodeRequest verifyCode)
    {
        // 从Redis中获取验证码
        var code = await _redis.GetStringAsync(verifyCode.Email);
        if (string.IsNullOrEmpty(code))
        {
            throw new BusinessException("验证码已过期或不存在");
        }

        // 验证验证码
        if (code != verifyCode.Code.Trim())
        {
            throw new BusinessException("验证码错误");
        }

        // 验证通过后，更新用户的邮箱验证状态
        var user = await _userManager.FindByEmailAsync(verifyCode.Email);
        if (user == null)
        {
            throw new BusinessException("用户不存在");
        }

        user.EmailConfirmed = true;
        user.Email = verifyCode.Email;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new Exception(string.Join(",", result.Errors.Select(e => e.Description)));
        }

        // 删除Redis中的验证码
        await _redis.RemoveAsync(verifyCode.Email);
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="resetPasswordRequest"></param>
    /// <returns></returns>
    public async Task ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest)
    {
        // 基本验证
        if (resetPasswordRequest == null || string.IsNullOrEmpty(resetPasswordRequest.Email) ||
            string.IsNullOrEmpty(resetPasswordRequest.ResetCode) ||
            string.IsNullOrEmpty(resetPasswordRequest.NewPassword))
        {
            throw new BadRequestException("请求参数不完整");
        }

        // 从Redis中获取验证码
        var code = await _redis.GetStringAsync(resetPasswordRequest.Email);
        if (string.IsNullOrEmpty(code))
        {
            throw new BusinessException("验证码已过期或不存在");
        }

        // 验证验证码
        if (code != resetPasswordRequest.ResetCode.Trim())
        {
            throw new BusinessException("验证码错误");
        }

        // 验证通过后，重置用户密码
        var user = await _userManager.FindByEmailAsync(resetPasswordRequest.Email);
        if (user == null)
        {
            throw new BusinessException("用户不存在");
        }

        // 使用内置方法重置密码
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordRequest.NewPassword);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(",", result.Errors.Select(e => e.Description)));
        }

        // 删除Redis中的验证码
        await _redis.RemoveAsync(resetPasswordRequest.Email);
    }

    /// <summary>
    /// 添加手机号
    /// </summary>
    /// <param name="verifyCode">验证码</param>
    /// <returns></returns>
    public async Task AddPhoneNumberAsync(VerifyCodeRequest verifyCode)
    {
        // 验证验证码
        bool isOk = await _smsService.VerifyCodeAsync(verifyCode.PhoneNumber, SmSPurposeEnum.ChangePhoneNumber,
            verifyCode.Code);
        if (isOk)
        {
            // 验证通过后，更新用户的手机号验证状态
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == verifyCode.PhoneNumber);
            if (user == null)
            {
                throw new BusinessException("用户不存在");
            }

            user.PhoneNumberConfirmed = true;
            user.PhoneNumber = verifyCode.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(",", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            throw new BusinessException("验证码错误");
        }
    }

    /// <summary>
    /// 短信验证登录
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="code"></param>
    /// <param name="scopes"></param>
    /// <returns>ClaimsPrincipal</returns>
    public async Task<ClaimsPrincipal> LoginBySmSCodeAsync(string phoneNumber, string code,
        ImmutableArray<string> scopes)
    {
        if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(code))
        {
            throw new BusinessException("手机号或验证码不能为空");
        }

        // 验证短信验证码
        bool isOk = await _smsService.VerifyCodeAsync(phoneNumber, SmSPurposeEnum.Login, code);
        if (!isOk)
        {
            throw new BusinessException("验证码错误");
        }

        // 查找用户
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        if (user == null)
        {
            throw new BusinessException("用户不存在");
        }

        return await BuildPrincipalAsync(user, scopes);
    }

    /// <summary>
    /// 邮箱验证码登录
    /// </summary>
    /// <param name="email"></param>
    /// <param name="code"></param>
    /// <param name="scopes"></param>
    /// <returns>ClaimsPrincipal</returns>
    public async Task<ClaimsPrincipal> LoginByEmailCodeAsync(string email, string code, ImmutableArray<string> scopes)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
        {
            throw new BusinessException("邮箱或验证码不能为空");
        }

        var redisCode = await _redis.GetStringAsync(email);
        if (string.IsNullOrEmpty(redisCode))
        {
            throw new BusinessException("验证码已过期或不存在");
        }
        if (redisCode != code.Trim())
        {
            throw new BusinessException("验证码错误");
        }
        // 查找用户
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new BusinessException("用户不存在");
        }

        await _redis.RemoveAsync(email);
        return await BuildPrincipalAsync(user, scopes);
    }

    /// <summary>
    /// 创建用户并分配默认角色，带事务
    /// </summary>
    /// <param name="newUser">即将创建的用户</param>
    /// <param name="password">可选密码（为空则不设置密码）</param>
    /// <param name="afterCommit">事务提交后的可选回调</param>
    /// <returns>用户ID</returns>
    private async Task<long> CreateUserWithDefaultRoleAsync(SpUser newUser, string? password = null,
        Func<Task>? afterCommit = null)
    {
        using var transaction = _dbContext.Database.BeginTransaction();
        try
        {
            IdentityResult result = password == null
                ? await _userManager.CreateAsync(newUser)
                : await _userManager.CreateAsync(newUser, password);
            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(newUser, "User");
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(newUser);
                    throw new Exception("用户创建成功，但分配角色失败：" +
                                        string.Join(",", roleResult.Errors.Select(e => e.Description)));
                }

                await transaction.CommitAsync();

                if (afterCommit != null)
                {
                    await afterCommit();
                }

                return newUser.Id;
            }

            throw new Exception(string.Join(",", result.Errors.Select(e => e.Description)));
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// 使用用户名注册
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="password">密码</param>
    /// <returns>用户ID</returns>
    private async Task<long> RegisterByUserNameAsync(string userName, string password)
    {
        // 检查userName是否存在
        var existingUser = await _userManager.FindByNameAsync(userName);
        if (existingUser != null)
        {
            throw new BusinessException("用户名已存在");
        }

        // 创建用户
        var newUser = new SpUser
        {
            Id = Snow.GetId(),
            UserName = userName,
        };
        return await CreateUserWithDefaultRoleAsync(newUser, password);
    }

    /// <summary>
    /// 使用邮箱注册
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="code">验证码</param>
    /// <returns>用户ID</returns>
    private async Task<long> RegisterByEmailAsync(string email, string code)
    {
        // 验证邮箱
        var emailUser = await _userManager.FindByEmailAsync(email);
        if (emailUser != null)
        {
            throw new BusinessException("邮箱已存在");
        }

        // 验证验证码
        var redisCode = await _redis.GetStringAsync(email);
        if (string.IsNullOrEmpty(redisCode))
        {
            throw new BusinessException("验证码已过期或不存在");
        }

        if (redisCode != code.Trim())
        {
            throw new BusinessException("验证码错误");
        }

        // 创建用户
        var newUser = new SpUser
        {
            Id = Snow.GetId(),
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };
        return await CreateUserWithDefaultRoleAsync(newUser, null, async () =>
        {
            // 删除Redis中的验证码
            await _redis.RemoveAsync(email);
        });
    }

    /// <summary>
    /// 使用手机号注册
    /// </summary>
    /// <param name="phoneNumber">手机号</param>
    /// <param name="code">验证码</param>
    /// <returns>用户ID</returns>
    private async Task<long> RegisterByPhoneNumberAsync(string phoneNumber, string code)
    {
        // 验证手机号
        var phoneUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        if (phoneUser != null)
        {
            throw new BusinessException("手机号已存在");
        }

        // 验证验证码
        bool isOk = await _smsService.VerifyCodeAsync(phoneNumber, SmSPurposeEnum.Register, code);
        if (!isOk)
        {
            throw new BusinessException("验证码错误");
        }

        // 创建用户
        var newUser = new SpUser
        {
            Id = Snow.GetId(),
            UserName = phoneNumber,
            PhoneNumber = phoneNumber,
            PhoneNumberConfirmed = true
        };
        return await CreateUserWithDefaultRoleAsync(newUser);
    }

    /// <summary>
    /// 通用生成ClaimsPrincipal方法
    /// </summary>
    /// <param name="user"></param>
    /// <param name="scopes"></param>
    /// <returns>ClaimsPrincipal</returns>
    private async Task<ClaimsPrincipal> BuildPrincipalAsync(SpUser user, ImmutableArray<string> scopes)
    {
        var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        identity.AddClaim(OpenIddictConstants.Claims.Subject, await _userManager.GetUserIdAsync(user));
        identity.AddClaim(OpenIddictConstants.Claims.Name, user.UserName ?? string.Empty);
        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            identity.AddClaim(OpenIddictConstants.Claims.Email, user.Email);
        }

        identity.AddClaim(OpenIddictConstants.Claims.Audience, "api");

        foreach (var role in await _userManager.GetRolesAsync(user))
        {
            identity.AddClaim(ClaimTypes.Role, role);
        }

        identity.SetDestinations(static claim => claim.Type switch
        {
            OpenIddictConstants.Claims.Name when claim.Subject.HasScope(OpenIddictConstants.Permissions.Scopes.Profile)
                => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],
            OpenIddictConstants.Claims.Email when claim.Subject.HasScope(OpenIddictConstants.Permissions.Scopes.Email)
                => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],
            ClaimTypes.Role => [OpenIddictConstants.Destinations.AccessToken],
            _ => [OpenIddictConstants.Destinations.AccessToken]
        });

        var principal = new ClaimsPrincipal(identity);

        var validScopes = new[] { "api", OpenIddictConstants.Scopes.OfflineAccess };
        var filtered = scopes.Intersect(validScopes).ToList();
        principal.SetScopes(filtered.Any() ? filtered : new[] { "api" });

        // 可根据角色/设备等动态设置生命周期（此处使用默认）
        principal.SetAccessTokenLifetime(TimeSpan.FromMinutes(30));
        principal.SetRefreshTokenLifetime(TimeSpan.FromDays(14));

        return principal;
    }
}