using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Server;
/// <summary>
/// URL服务实现
/// </summary>
public class SysUrlImp : ISysUrlServer
{
    private readonly SporeAccountingDBContext _dbContext;
    public SysUrlImp(SporeAccountingDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    /// <summary>
    /// 新增URL
    /// </summary>
    /// <param name="sysUrl"></param>
    public void Add(SysUrl sysUrl)
    {
        try
        {
            _dbContext.SysUrls.Add(sysUrl);
            _dbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 删除URL
    /// </summary>
    /// <param name="sysUrl"></param>
    public void Delete(SysUrl sysUrl)
    {
        try
        {
            _dbContext.SysUrls.Update(sysUrl);
            _dbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }
    /// <summary>
    /// 查询URL
    /// </summary>
    /// <param name="urlId"></param>
    public SysUrl Query(string urlId)
    {
        try
        {
            return _dbContext.SysUrls.FirstOrDefault(p => p.Id == urlId && !p.IsDeleted);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 查询URL列表
    /// </summary>
    /// <param name="sysRoleUrlPageViewModel"></param>
    /// <returns></returns>
    public (int rowCount, int pageCount, List<SysUrl> sysRoleUrls) GetByPage(
         SysUrlPageViewModel sysRoleUrlPageViewModel)
    {
        try
        {
            var query = _dbContext.SysUrls.Where(x => !x.IsDeleted);
            if (!string.IsNullOrEmpty(sysRoleUrlPageViewModel.Url))
            {
                query = query.Where(x => x.Url.Contains(sysRoleUrlPageViewModel.Url));
            }
            int rowCount = query.Count();
            int pageCount = (int)Math.Ceiling(rowCount / (double)sysRoleUrlPageViewModel.PageSize);
            List<SysUrl> sysUrls = query.Skip((sysRoleUrlPageViewModel.PageNumber - 1) * sysRoleUrlPageViewModel.PageSize)
                .Take(sysRoleUrlPageViewModel.PageSize).ToList();
            return (rowCount, pageCount, sysUrls);
        }
        catch (Exception e)
        {
            throw;
        }
    }
    /// <summary>
    /// 修改Url
    /// </summary>
    /// <param name="sysUrl"></param>
    public void Update(SysUrl sysUrl)
    {
        try
        {
            _dbContext.SysUrls.Update(sysUrl);
            _dbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }
    /// <summary>
    /// URL是否存在
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public bool IsExist(string url)
    {
        try
        {
            return _dbContext.SysUrls.Any(x => x.Url == url && !x.IsDeleted);
        }
        catch (Exception e)
        {
            throw;
        }
    }
    /// <summary>
    /// URL是否存在
    /// </summary>
    /// <param name="urlId"></param>
    /// <returns></returns>
    public bool IsExistById(string urlId)
    {
        try
        {
            return _dbContext.SysUrls.Any(x => x.Id == urlId && !x.IsDeleted);
        }
        catch (Exception e)
        {
            throw;
        }
    }
    /// <summary>
    /// 是否可以删除
    /// </summary>
    /// <param name="urlId"></param>
    /// <returns></returns>
    public bool CanDelete(string urlId)
    {
        try
        {
            return _dbContext.SysUrls.Any(x => x.Id == urlId && x.CanDelete);
        }
        catch (Exception e)
        {
            throw;
        }
    }
}