using SporeAccounting.Models;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Server;

/// <summary>
/// 用户实现类
/// </summary>
public class SysUserImp : ISysUserServer
{
    private SporeAccountingDBContext _dbContext;
    public SysUserImp(SporeAccountingDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    /// <summary>
    /// 新增用户
    /// </summary>
    /// <param name="sysUser">用户实体</param>
    public void Add(SysUser sysUser)
    {
        try
        {
            _dbContext.SysUsers.Add(sysUser);
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}