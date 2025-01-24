using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SporeAccounting.BaseModels;

namespace SporeAccounting.Models;

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
    [Required]
    [Column(TypeName = "int")]
    public int Month { get; set; }

    /// <summary>
    /// 报表名称
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public string Name { get; set; }

    /// <summary>
    /// 报表类型
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
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
    [Column(TypeName = "nvarchar(36)")]
    [ForeignKey("FK_Report_SysUser_UserId")]
    public string UserId { get; set; }

    /// <summary>
    /// 分类Id
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(36)")]
    [ForeignKey("FK_Report_Classification_ClassificationId")]
    public string ClassificationId { get; set; }

    /// <summary>
    /// 导航属性
    /// </summary>
    public SysUser User { get; set; }

    /// <summary>
    /// 导航属性
    /// </summary>
    public IncomeExpenditureClassification Classification { get; set; }
}