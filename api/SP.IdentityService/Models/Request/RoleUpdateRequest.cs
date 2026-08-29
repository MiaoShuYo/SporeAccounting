using System.ComponentModel.DataAnnotations;

namespace SP.IdentityService.Models.Request;

/// <summary>
/// 更新角色请求
/// </summary>
public class RoleUpdateRequest
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [Required(ErrorMessage = "角色名称不能为空")]
    [MaxLength(20, ErrorMessage = "角色名称不能超过20字")]
    public string RoleName { get; set; } = string.Empty;
}