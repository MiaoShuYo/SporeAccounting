using SporeAccounting.Models;

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
    /// <param name="sysUser"></param>
    SysUser Get(string userName);
    /// <summary>
    /// 修改用户
    /// </summary>
    /// <param name="sysUser"></param>
    void Update(SysUser sysUser);
}