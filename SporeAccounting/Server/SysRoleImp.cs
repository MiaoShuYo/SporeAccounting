using SporeAccounting.Models;
using SporeAccounting.Server.Interface;
using System.Data;

namespace SporeAccounting.Server;

public class SysRoleImp : ISysRoleServer
{

    private SporeAccountingDBContext _dbContext;

    public SysRoleImp(SporeAccountingDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    /// <summary>
    /// 新增角色
    /// </summary>
    /// <param name="role"></param>
    public void Add(SysRole role)
    {
        try
        {
            _dbContext.SysRoles.Add(role);
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 删除角色（逻辑）
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="userId"></param>
    public void Delete(string roleId, string userId)
    {
        try
        {
            SysRole role = _dbContext.SysRoles.FirstOrDefault(p => p.Id == roleId)!;
            role.IsDeleted = true;
            role.DeleteDateTime = DateTime.Now;
            role.DeleteUserId = userId;
            _dbContext.SysRoles.Update(role);
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 修改角色
    /// </summary>
    /// <param name="role"></param>
    public void Update(SysRole role)
    {
        try
        {
            SysRole dbRole = _dbContext.SysRoles.FirstOrDefault(p => p.Id == role.Id)!;
            dbRole.RoleName = role.RoleName;
            dbRole.UpdateDateTime = DateTime.Now;
            _dbContext.SysRoles.Update(role);
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 查询角色
    /// </summary>
    /// <param name="roleName"></param>
    public List<SysRole> Query(string roleName)
    {
        try
        {
            IQueryable<SysRole> sysRoles = _dbContext.SysRoles;
            if(!string.IsNullOrEmpty(roleName))
            {
                sysRoles = sysRoles.Where(p => p.RoleName.Contains(roleName));
            }
            return sysRoles.ToList();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 查询全部角色
    /// </summary>
    public List<SysRole> Query()
    {
        try
        {
            return _dbContext.SysRoles.Where(p => !p.IsDeleted).ToList();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    /// <summary>
    /// 根据名字查询角色
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public SysRole QueryByName(string roleName)
    {
        try
        {
            return _dbContext.SysRoles.FirstOrDefault(p => p.RoleName == roleName && !p.IsDeleted);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 角色是否存在
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public bool IsExistByRoleName(string roleName)
    {
        try
        {
            return _dbContext.SysRoles.Any(p => p.RoleName == roleName && !p.IsDeleted);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    /// <summary>
    /// 角色是否存在
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public bool IsExistById(string roleId)
    {
        try
        {
            return _dbContext.SysRoles.Any(p => p.Id == roleId && !p.IsDeleted);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 是否可以删除
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public bool CanDelete(string roleId)
    {
        try
        {
            return _dbContext.SysRoles.Any(p => p.Id == roleId && p.CanDelete && !p.IsDeleted);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    /// <summary>
    /// 角色是否重复
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public bool IsRepeat(string roleId, string roleName)
    {
        try
        {
            return _dbContext.SysRoles.Any(p => p.Id != roleId && p.RoleName == roleName && !p.IsDeleted);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}