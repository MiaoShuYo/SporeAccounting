using SporeAccounting.Models;

namespace SporeAccounting.Server.Interface;
/// <summary>
/// 角色数据库操作接口
/// </summary>
public interface ISysRoleServer
{
    /// <summary>
    /// 新增角色
    /// </summary>
    /// <param name="role"></param>
    void Add(SysRole role);
    /// <summary>
    /// 删除角色（逻辑）
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="userId"></param>
    void Delete(string roleId, string userId);
    /// <summary>
    /// 修改角色
    /// </summary>
    /// <param name="role"></param>
    void Update(SysRole role);
    /// <summary>
    /// 查询角色
    /// </summary>
    /// <param name="roleName"></param>
    List<SysRole> Query(string roleName);
    /// <summary>
    /// 角色是否存在
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    bool IsExistByRoleName(string roleName);
    /// <summary>
    /// 角色是否存在
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    bool IsExistById(string roleId);
    /// <summary>
    /// 角色是否重复
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    bool IsRepeat(string roleId, string roleName);
}