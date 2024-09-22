using System.ComponentModel.DataAnnotations;

namespace SporeAccounting.Models.ViewModels;

/// <summary>
/// 用户查询视图类
/// </summary>
public class SysUserQueryViewModel
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
    /// 创建时间
    /// </summary>
    public DateTime CreateDateTime { get; set; } 
}