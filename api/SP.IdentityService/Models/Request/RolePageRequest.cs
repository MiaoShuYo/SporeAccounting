namespace SP.IdentityService.Models.Request;

/// <summary>
/// 角色分页查询请求模型
/// </summary>
public class RolePageRequest: PageRequest
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleName { get; set; } = string.Empty;
}