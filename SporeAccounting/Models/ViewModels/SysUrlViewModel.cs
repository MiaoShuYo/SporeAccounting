using System.ComponentModel.DataAnnotations;

namespace SporeAccounting.Models.ViewModels;

public class SysUrlViewModel
{
    /// <summary>
    /// URL地址
    /// </summary>
    [MaxLength(200,ErrorMessage = "Url长度不能超过200")]
    [Required(ErrorMessage = "Url不能为空")]
    public string Url { get; set; }
    /// <summary>
    /// URL描述
    /// </summary>
    [MaxLength(200, ErrorMessage = "Description长度不能超过200")]
    public string Description { get; set; }
    /// <summary>
    /// 是否可删除
    /// </summary>
    [Required(ErrorMessage = "CanDelete不能为空")]
    public bool CanDelete { get; set; } = true;
}