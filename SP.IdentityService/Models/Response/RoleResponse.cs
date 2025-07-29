namespace SP.IdentityService.Models.Response;

/// <summary>
/// 用户响应
/// </summary>
public class RoleResponse
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleName { get; set; } = string.Empty;
}