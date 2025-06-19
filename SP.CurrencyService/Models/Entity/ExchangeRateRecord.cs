using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SP.Common;

namespace SP.CurrencyService.Models.Entity;

/// <summary>
/// 汇率记录
/// </summary>
[Table("ExchangeRateRecords")]
[Comment("汇率记录")]
public class ExchangeRateRecord : BaseModel
{
    /// <summary>
    /// 汇率
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    [Required]
    [Comment("汇率")]
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// 币种转换
    /// </summary>
    [Column(TypeName = "nvarchar(20)")]
    [Required]
    [Comment("币种转换")]
    public string ConvertCurrency { get; set; }

    /// <summary>
    /// 汇率日期
    /// </summary>
    [Column(TypeName = "date")]
    [Required]
    [Comment("汇率日期")]
    public DateTime Date { get; set; }
}