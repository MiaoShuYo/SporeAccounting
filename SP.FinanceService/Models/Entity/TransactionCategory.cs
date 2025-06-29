using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Common.Model;
using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Entity;

/// <summary>
/// 收支分类
/// </summary>
[Table(name: "TransactionCategory")]
public class TransactionCategory : BaseModel
{
    /// <summary>
    /// 类型名称
    /// </summary>
    [Required(ErrorMessage = "类型名称不能为空")]
    [MaxLength(20, ErrorMessage = "类型名称不能超过20字")]
    [Column(TypeName = "nvarchar(20)")]
    public string Name { get; set; }

    /// <summary>
    /// 收支类型
    /// </summary>
    [Required(ErrorMessage = "收支类型能为空")]
    [Column(TypeName = "int")]
    public TransactionCategoryEnmu Type { get; set; }

    /// <summary>
    /// 是否可以删除
    /// </summary>
    [Required(ErrorMessage = "是否可以删除")]
    [Column(TypeName = "tinyint(1)")]
    public bool CanDelete { get; set; } = true;

    /// <summary>
    /// 父级分类ID
    /// </summary>
    [Column(TypeName = "nvarchar(36)")]
    [ForeignKey("ParentIncomeExpenditureClassification")]
    public string? TransactionCategoryId { get; set; }
}