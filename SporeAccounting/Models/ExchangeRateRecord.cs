using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SporeAccounting.BaseModels;

namespace SporeAccounting.Models;

/// <summary>
/// 汇率记录表
/// </summary>
[Table(name: "ExchangeRate")]
public class ExchangeRateRecord : BaseModel
{
    /// <summary>
    /// 汇率
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    [Required]
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// 币种转换
    /// </summary>
    [Column(TypeName = "nvarchar(20)")]
    [Required]
    public string ConvertCurrency { get; set; }

    /// <summary>
    /// 汇率日期
    /// </summary>
    [Column(TypeName = "date")]
    [Required]
    public DateTime Date { get; set; }
}