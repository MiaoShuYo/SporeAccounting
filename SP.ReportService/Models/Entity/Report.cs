using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Common.Model;
using SP.ReportService.Models.Enumeration;

namespace SP.ReportService.Models.Entity;

/// <summary>
/// 报表
/// </summary>
[Table("Report")]
public class Report : BaseModel
{
    /// <summary>
    /// 年份
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public int Year { get; set; }

    /// <summary>
    /// 月份
    /// </summary>
    [Column(TypeName = "int")]
    public int? Month { get; set; }
    /// <summary>
    /// 季度
    /// </summary>
    [Column(TypeName = "int")]
    public int? Quarter { get; set; }

    /// <summary>
    /// 报表名称
    /// </summary>
    [Required]
    [Column(TypeName = "varchar(100)")]
    public string Name { get; set; }

    /// <summary>
    /// 报表类型
    /// </summary>
    [Required]
    [Column(TypeName = "tinyint")]
    public ReportTypeEnum Type { get; set; }

    /// <summary>
    /// 金额
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public long UserId { get; set; }

    /// <summary>
    /// 分类Id
    /// </summary>
    [Required]
    [Column(TypeName = "bigint")]
    public long TransactionCategoryId { get; set; }
}