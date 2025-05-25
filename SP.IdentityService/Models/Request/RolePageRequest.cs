namespace SP.IdentityService.Models.Request;

/// <summary>
/// 分页请求基类
/// </summary>
public class RolePageRequest: PageRequest
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleName { get; set; } = string.Empty;
}