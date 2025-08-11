using System.ComponentModel.DataAnnotations;
using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Request;

/// <summary>
/// 收支分类添加请求模型
/// </summary>
public class TransactionCategoryAddRequest
{
    /// <summary>
    /// 分类名称
    /// </summary>
    [Required(ErrorMessage = "分类名称不能为空")]
    [MaxLength(20, ErrorMessage = "分类名称不能超过20字")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 父级分类ID
    /// </summary>
    public long? ParentId { get; set; }
    
    /// <summary>
    /// 收支类型
    /// </summary>
    public TransactionCategoryEnmu Type { get; set; }
    
}