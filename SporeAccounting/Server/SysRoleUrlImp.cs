﻿using Microsoft.EntityFrameworkCore;
using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Server;
/// <summary>
/// 角色可访问的URL数据库操作
/// </summary>
public class SysRoleUrlImp : ISysRoleUrlServer
{
    private SporeAccountingDBContext _dbContext;

    public SysRoleUrlImp(SporeAccountingDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// 新增角色可访问的URL
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="url"></param>
    public void Add(SysRoleUrl roleUrl)
    {
        try
        {
            _dbContext.SysRoleUrls.Add(roleUrl);
            _dbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }
    /// <summary>
    /// 删除角色可访问的URL
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="urlId"></param>
    public void Delete(string roleId, string urlId)
    {
        try
        {
            SysRoleUrl roleUrl = _dbContext.SysRoleUrls.
                FirstOrDefault(x => x.RoleId == roleId && x.UrlId == urlId);
            if (roleUrl != null)
            {
                _dbContext.SysRoleUrls.Remove(roleUrl);
                _dbContext.SaveChanges();
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 修改角色可访问的URL
    /// </summary>
    /// <param name="id"></param>
    /// <param name="roleId"></param>
    /// <param name="urlId"></param>
    public void Edit(string id, string roleId, string urlId)
    {
        try
        {
            SysRoleUrl roleUrl = _dbContext.SysRoleUrls.FirstOrDefault(x => x.Id == id);
            if (roleUrl != null)
            {
                roleUrl.UrlId = urlId;
                roleUrl.RoleId = roleId;
                _dbContext.SysRoleUrls.Update(roleUrl);
                _dbContext.SaveChanges();
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }
    /// <summary>
    /// 角色可访问的URL是否存在
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="url"></param>
    /// <returns></returns>
    public bool IsExist(string roleId, string urlId)
    {
        try
        {
            return _dbContext.SysRoleUrls.Any(x => x.RoleId == roleId && x.UrlId == urlId);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 角色是否可以访问URL
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="url"></param>
    /// <returns></returns>
    public bool IsRoleUseUrl(string roleId, string url)
    {
        try
        {
            return _dbContext.SysRoleUrls.Any(x => x.RoleId == roleId && x.Url.Url == url);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 是否可以删除
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="urlId"></param>
    /// <returns></returns>
    public bool IsDelete(string roleId, string urlId)
    {
        try
        {
            return _dbContext.SysRoleUrls
                .Any(x => x.RoleId == roleId && x.UrlId == urlId && x.CanDelete);
        }
        catch (Exception e)
        {
            throw;
        }
    }
    /// <summary>
    /// 查询角色可访问的URL
    /// </summary>
    /// <param name="roleId"></param>
    public List<SysRoleUrlInfoVideModel> Query(string roleId)
    {
        try
        {
            return _dbContext.SysRoleUrls
                .Where(x => x.RoleId == roleId)
                .Select(x => new SysRoleUrlInfoVideModel()
                {
                    RoleId = x.RoleId,
                    UrlId = x.UrlId,
                    RoleName = x.Role.RoleName,
                    Url = x.Url.Url
                }).ToList();
        }
        catch (Exception e)
        {
            throw;
        }
    }
    /// <summary>
    /// 分页查询角色可访问的URL
    /// </summary>
    /// <param name="sysRoleUrlPageViewModel"></param>
    /// <returns></returns>
    public (int rowCount, int pageCount, List<SysRoleUrlInfoVideModel> sysRoleUrls) GetByPage(SysRoleUrlPageViewModel sysRoleUrlPageViewModel)
    {
        try
        {
            //计算跳过的数据量
            int skip = (sysRoleUrlPageViewModel.PageNumber - 1) * sysRoleUrlPageViewModel.PageSize;
            IQueryable<SysRoleUrlInfoVideModel> roleUrls = _dbContext.SysRoleUrls
                .Select(s => new SysRoleUrlInfoVideModel()
                {
                    RoleId = s.RoleId,
                    RoleName = s.Role.RoleName,
                    UrlId = s.UrlId,
                    Url = s.Url.Url
                });
            if (!string.IsNullOrEmpty(sysRoleUrlPageViewModel.RoleName))
            {
                roleUrls = roleUrls.Where(ru => ru.RoleName.Contains(sysRoleUrlPageViewModel.RoleName));
            }
            if (!string.IsNullOrEmpty(sysRoleUrlPageViewModel.Url))
            {
                roleUrls = roleUrls.Where(ru => ru.Url.Contains(sysRoleUrlPageViewModel.Url));
            }
            //总行数
            int rowCount = roleUrls.Count();
            //总页数
            int pageCount = (int)Math.Ceiling(rowCount / (double)sysRoleUrlPageViewModel.PageSize);
            var roleUrlsList = roleUrls.Skip(skip).Take(sysRoleUrlPageViewModel.PageSize).ToList();
            return (rowCount, pageCount, roleUrlsList);
        }
        catch (Exception e)
        {
            throw;
        }
    }
}