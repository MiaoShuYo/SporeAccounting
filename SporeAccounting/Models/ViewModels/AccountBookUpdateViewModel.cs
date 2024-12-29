using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SporeAccounting.Models.ViewModels;
/// <summary>
/// 账本更新视图模型
/// </summary>
public class AccountBookUpdateViewModel
{
    /// <summary>
    /// 账本ID
    /// </summary>
    [Required(ErrorMessage = "账本ID不能为空")]
    [MaxLength(36)]
    public string AccountBookId { get; set; }

    /// <summary>
    /// 账本名称
    /// </summary>
    [Required(ErrorMessage = "账本名称不能为空")]
    [MaxLength(20)]
    public string Name { get; set; }

    /// <summary>
    /// 账簿余额
    /// </summary>
    [Required(ErrorMessage = "账簿余额不能为空")]
    public decimal Balance { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(100)]
    public string? Remarks { get; set; }
}