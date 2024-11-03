using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;

namespace SporeAccounting.Server.Interface;
/// <summary>
/// URL服务接口
/// </summary>
public interface ISysUrlServer
{
    /// <summary>
    /// 新增URL
    /// </summary>
    /// <param name="sysUrl"></param>
    void Add(SysUrl sysUrl);
    /// <summary>
    /// 删除URL
    /// </summary>
    /// <param name="urlId"></param>
    void Delete(SysUrl sysUrl);
    /// <summary>
    /// 查询URL
    /// </summary>
    /// <param name="urlId"></param>
    SysUrl Query(string urlId);
    /// <summary>
    /// 查询URL列表
    /// </summary>
    /// <param name="sysRoleUrlPageViewModel"></param>
    /// <returns></returns>
    (int rowCount, int pageCount, List<SysUrl> sysRoleUrls) GetByPage(SysUrlPageViewModel sysRoleUrlPageViewModel);
    /// <summary>
    /// 修改Url
    /// </summary>
    /// <param name="sysUrl"></param>
    void Update(SysUrl sysUrl);
    /// <summary>
    /// URL是否存在
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    bool IsExist(string url);
    /// <summary>
    /// URL是否存在
    /// </summary>
    /// <param name="urlId"></param>
    /// <returns></returns>
    bool IsExistById(string urlId);
    /// <summary>
    /// 是否可以删除
    /// </summary>
    /// <param name="urlId"></param>
    /// <returns></returns>
    bool CanDelete(string urlId);
}