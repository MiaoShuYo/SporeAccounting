using System.ComponentModel.DataAnnotations;

namespace SP.IdentityService.Models.Request;

/// <summary>
/// 创建角色请求模型
/// </summary>
public class RoleCreateRequest
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [Required(ErrorMessage = "角色名称不能为空")]
    [MaxLength(20, ErrorMessage = "角色名称不能超过20字")]
    public string RoleName { get; set; } = string.Empty;
}