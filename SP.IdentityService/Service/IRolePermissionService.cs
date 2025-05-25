namespace SP.IdentityService;

/// <summary>
/// 角色权限服务接口
/// </summary>
public interface IRolePermissionService
{
    Task AddPermissionToRole(long roleId, string permission);
    Task<List<string>> GetPermissionsByRole(long roleId);
    Task RemovePermissionFromRole(long roleId, string permission);
    Task<List<string>> GetUserPermissions(long userId);
}