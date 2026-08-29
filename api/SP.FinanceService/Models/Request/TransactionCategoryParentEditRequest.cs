using System.ComponentModel.DataAnnotations;

namespace SP.FinanceService.Models.Request;

/// <summary>
/// 收支分类父级编辑请求模型
/// </summary>
public class TransactionCategoryParentEditRequest
{
    /// <summary>
    /// 分类ID集合
    /// </summary>
    [Required(ErrorMessage = "分类ID不能为空")]
    public List<long>? Id { get; set; }

    /// <summary>
    /// 父级分类ID
    /// </summary>
    [Required(ErrorMessage = "父级分类ID不能为空")]
    public long ParentId { get; set; }
}