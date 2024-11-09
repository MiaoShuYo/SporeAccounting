using System.ComponentModel.DataAnnotations;

namespace SporeAccounting.Models.ViewModels;

public class SysRoleUrlViewModel
{
    /// <summary>
    /// RoleUrl Id
    /// </summary>
    [MaxLength(36)]
    [Required(ErrorMessage = "Id不能为空")]
    public string Id { get; set; }
    /// <summary>
    /// 角色id
    /// </summary>
    [MaxLength(36)]
    [Required(ErrorMessage = "角色Id不能为空")]
    public string RoleId { get; set; }
    /// <summary>
    /// Url Id
    /// </summary>
    [MaxLength(36)]
    [Required(ErrorMessage = "UrlId不能为空")]
    public string UrlId { get; set; }
    /// <summary>
    /// 是否可以删除
    /// </summary>
    [MaxLength(36)]
    [Required(ErrorMessage = "CanDelete不能为空")]
    public bool CanDelete { get; set; } = true;

}