namespace SporeAccounting.Models.ViewModels;

/// <summary>
/// 用户信息视图模型
/// </summary>
public class SysUserInfoViewModel
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// 用户配置信息
    /// </summary>
    public List<ConfigInfoViewModel> Configs { get; set; }
}