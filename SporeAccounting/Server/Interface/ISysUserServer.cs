using SporeAccounting.BaseModels.ViewModel.Response;
using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;

namespace SporeAccounting.Server.Interface;

/// <summary>
/// 用户接口
/// </summary>
public interface ISysUserServer
{
    /// <summary>
    /// 新增用户
    /// </summary>
    void Add(SysUser sysUser);
    /// <summary>
    /// 根据用户名获取用户
    /// </summary>
    /// <param name="userName"></param>
    SysUser GetByUserName(string userName);

    /// <summary>
    /// 根据用户id获取用户
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    SysUser? GetById(string userId);
    /// <summary>
    /// 修改用户
    /// </summary>
    /// <param name="sysUser"></param>
    void Update(SysUser sysUser);
    /// <summary>
    /// 分页查询用户
    /// </summary>
    /// <param name="userPage"></param>
    (int rowCount, int pageCount, List<SysUser> sysUsers) GetByPage(UserPageViewModel userPage);
    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="userId"></param>
    void Delete(string userId);
}