using SP.IdentityService.Models.Request;
using SP.IdentityService.Models.Response;

namespace SP.IdentityService.Service;

/// <summary>
/// 用户服务接口
/// </summary>
public interface IUserService
{
    Task<UserResponse?> GetUserInfo(long id);
    Task<PagedResponse<UserResponse>> GetUserList(UserPageRequest page);
    Task DeleteUser(long id);
    Task DisableUser(long id, bool isDisabled);
    Task UpdateUser(long id, UserUpdateRequest user);
}