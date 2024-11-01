using SporeAccounting.BaseModels.ViewModel.Request;

namespace SporeAccounting.Models.ViewModels;

public class SysUserPageViewModel: PageRequestViewModel
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get; set; }
}