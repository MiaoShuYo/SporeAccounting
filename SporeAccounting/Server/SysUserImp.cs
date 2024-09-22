using Microsoft.EntityFrameworkCore;
using SporeAccounting.BaseModels.ViewModel.Response;
using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;
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

    /// <summary>
    /// 根据用户名获取用户
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    public SysUser GetByUserName(string userName)
    {
        try
        {
            SysUser sysUser = _dbContext.SysUsers.FirstOrDefault(p => p.UserName == userName);
            return sysUser;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 根据用户Id获取用户
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public SysUser? GetById(string userId)
    {
        try
        {
            SysUser sysUser = _dbContext.SysUsers.FirstOrDefault(p => p.Id == userId);
            return sysUser;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    /// <summary>
    /// 修改用户
    /// </summary>
    /// <param name="sysUser"></param>
    public void Update(SysUser sysUser)
    {
        try
        {
            _dbContext.SysUsers.Update(sysUser);
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 分页查询用户
    /// </summary>
    /// <param name="userPage"></param>
    /// <returns></returns>
    public (int rowCount, int pageCount, List<SysUser> sysUsers) GetByPage(UserPageViewModel userPage)
    {
        try
        {
            //计算跳过的数据量
            int skip = (userPage.PageNumber - 1) * userPage.PageSize;
            var users = _dbContext.SysUsers
                .Where(p => !p.IsDeleted);

            if (!string.IsNullOrEmpty(userPage.UserName))
            {
                users = users
                    .Where(p =>
                        p.UserName.Contains(userPage.UserName));
            }
            //总行数
            int rowCount = 0;
            //总页数
            int pageCount = 0;
            var usersList = users.ToList();
            rowCount = usersList.Count();
            pageCount = rowCount % userPage.PageSize == 0
                ? rowCount / userPage.PageSize
                : (rowCount / userPage.PageSize) + 1;

            users = _dbContext.SysUsers
                .OrderByDescending(p => p.CreateDateTime)
                .Skip(skip)
                .Take(userPage.PageSize);
          
            return (rowCount, pageCount, usersList);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    /// <summary>
    /// 删除用户（逻辑删除）
    /// </summary>
    /// <param name="userId"></param>
    public void Delete(string userId)
    {
        try
        {
            var sysUser = _dbContext.SysUsers.FirstOrDefault(p => p.Id == userId);
            sysUser.IsDeleted = true;
            sysUser.DeleteDateTime= DateTime.Now;
            _dbContext.SysUsers.Update(sysUser);
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}