using SporeAccounting.BaseModels.ViewModel.Request;

namespace SporeAccounting.Models.ViewModels;

/// <summary>
/// 收支记录分页视图模型
/// </summary>
public class IncomeExpenditureRecordPageViewModel : PageRequestViewModel
{
    /// <summary>
    /// 开始日期
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    public DateTime EndDate { get; set; }
}