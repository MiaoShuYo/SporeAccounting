using System.ComponentModel.DataAnnotations;

namespace SporeAccounting.Models.ViewModels;

public class SysRoleUrlViewModel
{
    [MaxLength(36)]
    [Required(ErrorMessage = "Id不能为空")]
    public string Id { get; set; }
    [MaxLength(36)]
    [Required(ErrorMessage = "角色Id不能为空")]
    public string RoleId { get; set; }
    [MaxLength(100)]
    [Required(ErrorMessage = "Url不能为空")]
    public string Url { get; set; }
}