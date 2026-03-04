using Refit;
using SP.Common.Model;
using SP.NotificationService.Models.Request;
using SP.NotificationService.Models.Response;

namespace SP.NotificationService.RefitClient;

/// <summary>
/// 身份服务 Refit 客户端接口
/// </summary>
public interface IIdentityServiceApi
{
    /// <summary>
    /// 获取用户分页列表
    /// </summary>
    /// <param name="request">分页请求</param>
    /// <returns>用户分页列表</returns>
    [Get("/api/users")]
    Task<ApiResponse<PageResponse<UserResponse>>> GetUsers([Query] UserPageRequest request);
}
