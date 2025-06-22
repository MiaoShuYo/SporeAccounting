using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Common.Model;

namespace SP.CurrencyService.Models.Entity;


/// <summary>
/// 币种
/// </summary>
[Table(name: "Currency")]
public class Currency : BaseModel
{
    /// <summary>
    /// 币种名称
    /// </summary>
    [Column(TypeName = "nvarchar(20)")]
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// 币种缩写
    /// </summary>
    [Column(TypeName = "nvarchar(10)")]
    [Required]
    public string Abbreviation { get; set; }
}