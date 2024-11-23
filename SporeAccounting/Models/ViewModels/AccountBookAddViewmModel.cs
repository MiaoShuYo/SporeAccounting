using System.ComponentModel.DataAnnotations;

namespace SporeAccounting.Models.ViewModels;

/// <summary>
/// 账簿新增视图模型
/// </summary>
public class AccountBookAddViewmModel
{
    /// <summary>
    /// 账簿名称
    /// </summary>
    [Required(ErrorMessage = "账簿名称不能为空")]
    [MaxLength(20, ErrorMessage = "账簿名称不能超过20字")]
    public string Name { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(100)]
    public string? Remarks { get; set; }
}