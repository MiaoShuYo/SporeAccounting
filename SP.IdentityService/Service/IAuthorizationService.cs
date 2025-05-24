using System.Collections.Immutable;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.Data;
using SP.IdentityService.Models.Request;

namespace SP.IdentityService.Service;

/// <summary>
/// 用户服务接口
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// 密码登录
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="scopes"></param>
    /// <returns></returns>
    Task<ClaimsPrincipal> LoginByPasswordAsync(string userName, string password, ImmutableArray<string> scopes);

    /// <summary>
    /// 刷新token
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="scopes"></param>
    /// <param name="principal"></param>
    /// <returns></returns>
    Task<ClaimsPrincipal> RefreshTokenAsync(string? refreshToken, ImmutableArray<string> scopes,
        ClaimsPrincipal? principal);

    /// <summary>
    /// 处理客户端凭证模式
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="scopes"></param>
    /// <returns></returns>
    Task<ClaimsPrincipal> HandleClientCredentialsAsync(string clientId, ImmutableArray<string> scopes);

    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="user">用户添加请求</param>
    /// <returns>用户id</returns>
    Task<long> AddUserAsync(UserAddRequest user);

    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="email"></param>
    /// <returns>是否发送成功</returns>
    Task SendEmailAsync(SendEmailRequest email);

    /// <summary>
    /// 添加邮箱
    /// </summary>
    /// <param name="verifyCode"></param>
    /// <returns></returns>
    Task AddEmailAsync(VerifyCodeRequest verifyCode);

    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="resetPasswordRequest"></param>
    /// <returns></returns>
    Task ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest);
}