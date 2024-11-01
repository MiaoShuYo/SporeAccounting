using SporeAccounting.BaseModels.ViewModel.Request;

namespace SporeAccounting.Models.ViewModels;
/// <summary>
/// 角色可访问的Url分页查询视图模型
/// </summary>
public class SysRoleUrlPageViewModel: PageRequestViewModel
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleName { get; set; }
    /// <summary>
    /// 角色可访问的Url
    /// </summary>
    public string Url { get; set; }
}