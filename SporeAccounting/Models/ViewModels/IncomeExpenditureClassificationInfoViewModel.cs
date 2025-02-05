using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SporeAccounting.Models.ViewModels;

/// <summary>
/// 收支分类信息视图模型
/// </summary>
public class IncomeExpenditureClassificationInfoViewModel
{
    /// <summary>
    /// 收支分类Id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 收支类型
    /// </summary>
    public IncomeExpenditureTypeEnmu Type { get; set; }

    /// <summary>
    /// 父级id
    /// </summary>
    public string ParentId { get; set; }

    /// <summary>
    /// 父级名称
    /// </summary>
    public string ParentName { get; set; }
}