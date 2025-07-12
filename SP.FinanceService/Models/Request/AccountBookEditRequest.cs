using System.ComponentModel.DataAnnotations;

namespace SP.FinanceService.Models.Request;

/// <summary>
/// 账本修改请求模型
/// </summary>
public class AccountBookEditeRequest
{
    /// <summary>
    /// 账本ID
    /// </summary>
    [Required(ErrorMessage = "账本ID不能为空")]
    public long Id { get; set; }

    /// <summary>
    /// 账簿名称
    /// </summary>
    [StringLength(20, ErrorMessage = "账簿名称长度不能超过20个字符")]
    [Required(ErrorMessage = "账簿名称不能为空")]
    public string Name { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(100, ErrorMessage = "备注长度不能超过100个字符")]
    public string? Remarks { get; set; }
}