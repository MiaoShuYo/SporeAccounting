using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Quartz.Xml.JobSchedulingData20;
using SporeAccounting.BaseModels;

namespace SporeAccounting.Models;

/// <summary>
/// 收支记录表
/// </summary>
[Table("IncomeExpenditureRecord")]
public class IncomeExpenditureRecord : BaseModel
{
    //记录内容包括转换前金额、转换后金额、分类、日期、转换前币种、备注
    /// <summary>
    /// 转换前金额
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    [Required]
    public decimal BeforAmount { get; set; }

    /// <summary>
    /// 转换后金额
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    [Required]
    public decimal AfterAmount { get; set; }

    /// <summary>
    /// 收支分类Id
    /// </summary>
    [Column(TypeName = "nvarchar(36)")]
    [ForeignKey("FK_IncomeExpenditureRecord_IncomeExpenditureClassification")]
    [Required]
    public string IncomeExpenditureClassificationId { get; set; }

    /// <summary>
    /// 转换前币种Id
    /// </summary>
    [Column(TypeName = "nvarchar(36)")]
    [ForeignKey("FK_IncomeExpenditureRecord_Currency")]
    [Required]
    public string CurrencyId { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    public string? Remark { get; set; }

    /// <summary>
    /// 用户id
    /// </summary>
    [Column(TypeName = "nvarchar(36)")]
    [ForeignKey("FK_IncomeExpenditureRecord_SysUser")]
    [Required]
    public string UserId { get; set; }

    /// <summary>
    /// 导航属性
    /// </summary>
    public SysUser User { get; set; }

    /// <summary>
    /// 导航属性
    /// </summary>
    public Currency Currency { get; set; }

    /// <summary>
    /// 导航属性
    /// </summary>
    public IncomeExpenditureClassification IncomeExpenditureClassification { get; set; }
}