using System.ComponentModel.DataAnnotations;

namespace SporeAccounting.Models.ViewModels;

public class SysUrlViewModel
{
    /// <summary>
    /// URL地址
    /// </summary>
    [MaxLength(200, ErrorMessage = "Url长度不能超过200")]
    [Required(ErrorMessage = "Url不能为空")]
    public string Url { get; set; }

    /// <summary>
    /// 请求方法
    /// </summary>
    [MaxLength(10, ErrorMessage = "请求方法长度不能超过10个字符")]
    [Required(ErrorMessage = "请求方法不能为空")]
    public string RequestMethod { get; set; }

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