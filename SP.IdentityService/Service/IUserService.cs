using System.Collections.Immutable;
using System.Security.Claims;
using SP.IdentityService.Models.Request;

namespace SP.IdentityService.Service;

/// <summary>
/// 用户服务接口
/// </summary>
public interface IUserService
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
}