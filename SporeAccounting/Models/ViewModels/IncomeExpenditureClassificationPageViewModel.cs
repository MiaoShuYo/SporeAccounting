using SporeAccounting.BaseModels.ViewModel.Request;

namespace SporeAccounting.Models.ViewModels;
/// <summary>
/// 收支分类视图模型
/// </summary>
public class IncomeExpenditureClassificationPageViewModel: PageRequestViewModel
{
    /// <summary>
    /// 分类名称
    /// </summary>
    public string? ClassificationName { get; set; }
    /// <summary>
    /// 收支类型
    /// </summary>
    public IncomeExpenditureTypeEnmu? Type { get; set; }
    /// <summary>
    /// 父级分类ID
    /// </summary>
    public string? ParentClassificationId { get; set; }
}