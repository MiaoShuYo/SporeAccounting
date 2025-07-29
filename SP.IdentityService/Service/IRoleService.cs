using SP.IdentityService.Models.Request;
using SP.IdentityService.Models.Response;

namespace SP.IdentityService.Service;

/// <summary>
/// 角色服务接口
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// 获取角色列表
    /// </summary>
    Task<PagedResponse<RoleResponse>> GetRoleList(RolePageRequest page);

    /// <summary>
    /// 获取角色信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<RoleResponse> GetRoleInfo(long id);

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    Task CreateRole(RoleCreateRequest role);

    /// <summary>
    /// 更新角色
    /// </summary>
    /// <param name="id"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    Task UpdateRole(long id, RoleUpdateRequest role);

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteRole(long id);
}