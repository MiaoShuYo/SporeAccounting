namespace SP.IdentityService.Models.Request;

/// <summary>
/// 更新角色请求
/// </summary>
public class RoleUpdateRequest
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleName { get; set; } = string.Empty;
}