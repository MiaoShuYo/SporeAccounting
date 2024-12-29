using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;

namespace SporeAccounting.Server.Interface;

/// <summary>
/// 角色可访问的URL数据库操作接口
/// </summary>
public interface ISysRoleUrlServer
{
    /// <summary>
    /// 新增角色可访问的URL
    /// </summary>
    /// <param name="roleUrl"></param>
    void Add(SysRoleUrl roleUrl);
    /// <summary>
    /// 批量新增角色可访问的URL
    /// </summary>
    /// <param name="roleUrls"></param>
    void Add(List<SysRoleUrl> roleUrls);
    /// <summary>
    /// 删除角色可访问的URL
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="urlId"></param>
    void Delete(string roleId, string urlId);
    /// <summary>
    /// 修改角色可访问的URL
    /// </summary>
    /// <param name="id"></param>
    /// <param name="roleId"></param>
    /// <param name="url"></param>
    void Edit(string id, string roleId, string urlId);
    /// <summary>
    /// 查询角色可访问的URL
    /// </summary>
    /// <param name="roleId"></param>
    List<SysRoleUrlInfoVideModel> Query(string roleId);
    /// <summary>
    /// 分页查询角色可访问的URL
    /// </summary>
    /// <param name="sysRoleUrlPageViewModel"></param>
    /// <returns></returns>
    (int rowCount, int pageCount, List<SysRoleUrlInfoVideModel> sysRoleUrls) GetByPage(SysRoleUrlPageViewModel sysRoleUrlPageViewModel);
    /// <summary>
    /// 角色可访问的URL是否存在
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="urlId"></param>
    /// <returns></returns>
    bool IsExist(string roleId, string urlId);
    /// <summary>
    /// 角色是否可以访问URL
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="url"></param>
    /// <returns></returns>
    bool IsRoleUseUrl(string roleId, string url);

    /// <summary>
    /// 是否可以删除
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="urlId"></param>
    /// <returns></returns>
    bool IsDelete(string roleId, string urlId);
}