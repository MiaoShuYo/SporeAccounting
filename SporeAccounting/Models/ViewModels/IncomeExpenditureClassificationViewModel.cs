using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SporeAccounting.Models.ViewModels;

public class IncomeExpenditureClassificationViewModel
{
    /// <summary>
    /// 分类名称
    /// </summary>
    [Required(ErrorMessage = "分类名称不能为空")]
    [MaxLength(20)]
    public string Name { get; set; }

    /// <summary>
    /// 收支类型
    /// </summary>
    [Required(ErrorMessage = "收支类型不能为空")]
    [EnumDataType(typeof(IncomeExpenditureTypeEnmu))]
    [JsonConverter(typeof(IncomeExpenditureTypeEnmuConverter))]
    public IncomeExpenditureTypeEnmu Type { get; set; }

    /// <summary>
    /// 腹肌分类Id
    /// </summary>
    [MaxLength(36)]
    public string? ParentClassificationId { get; set; }
}