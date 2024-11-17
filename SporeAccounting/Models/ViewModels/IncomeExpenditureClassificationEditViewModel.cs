using System.ComponentModel.DataAnnotations;

namespace SporeAccounting.Models.ViewModels;

public class IncomeExpenditureClassificationEditViewModel
{
    /// <summary>
    /// 分类Id
    /// </summary>
    [MaxLength(36)]
    [Required(ErrorMessage = "分类Id不能为空")]
    public string Id { get; set; }
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
    public IncomeExpenditureTypeEnmu Type { get; set; }

    /// <summary>
    /// 腹肌分类Id
    /// </summary>
    [MaxLength(36)]
    public string? ParentClassificationId { get; set; }
}