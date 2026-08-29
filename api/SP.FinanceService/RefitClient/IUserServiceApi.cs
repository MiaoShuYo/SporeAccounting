using Refit;
using SP.Common.Model;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.RefitClient;

/// <summary>
/// 人员接口
/// </summary>
public interface IUserServiceApi
{
    /// <summary>
    /// 根据ID获取用户信息
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>返回用户信息</returns>
    [Get("/api/users/{id}")]
    Task<ApiResponse<UserResponse>> GetUser(long id);

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <param name="request">分页请求</param>
    /// <returns>返回用户列表</returns>
    [Get("/api/users")]
    Task<ApiResponse<PageResponse<UserResponse>>> GetUsers([Query] UserPageRequest request);
}