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
    [Required]
    [MaxLength(20)]
    [Column(TypeName = "nvarchar(20)")]
    public string Name { get; set; }

    /// <summary>
    /// 收支类型
    /// </summary>
    [Required]
    [Column(TypeName = "tinyint")]
    public TransactionCategoryEnmu Type { get; set; }

    /// <summary>
    /// 是否可以删除
    /// </summary>
    [Required]
    [Column(TypeName = "tinyint(1)")]
    public bool CanDelete { get; set; } = true;

    /// <summary>
    /// 父级分类ID
    /// </summary>
    [Column(TypeName = "bigint")]
    public long? ParentId { get; set; }
}