using System.ComponentModel.DataAnnotations;

namespace SP.FinanceService.Models.Request;

/// <summary>
/// 收支分类编辑请求模型
/// </summary>
public class TransactionCategoryEditRequest
{
    /// <summary>
    /// 分类ID
    /// </summary>
    [Required(ErrorMessage = "分类ID不能为空")]
    public long Id { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    [Required(ErrorMessage = "分类名称不能为空")]
    [MaxLength(20, ErrorMessage = "分类名称不能超过20字")]
    public string Name { get; set; } = string.Empty;
}