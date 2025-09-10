using SP.IdentityService.Models.Request;
using SP.IdentityService.Models.Response;

namespace SP.IdentityService.Service;

/// <summary>
/// 用户服务接口
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<UserResponse?> GetUserInfo(long id);

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    Task<PagedResponse<UserResponse>> GetUserList(UserPageRequest page);

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteUser(long id);

    /// <summary>
    /// 禁用或启用用户
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isDisabled"></param>
    /// <returns></returns>
    Task DisableUser(long id, bool isDisabled);

    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    Task UpdateUser(long id, UserUpdateRequest user);
    
    /// <summary>
    /// 查询用户手机是否已验证
    /// </summary>
    /// <returns></returns>
    Task<bool> IsUserPhoneVerified();
    
    /// <summary>
    /// 查询用户邮箱是否已验证
    /// </summary>
    /// <returns></returns>
    Task<bool> IsUserEmailVerified();
}