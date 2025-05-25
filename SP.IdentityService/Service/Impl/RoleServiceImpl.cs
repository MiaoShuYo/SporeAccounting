using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SP.Common.ExceptionHandling.Exceptions;
using SP.IdentityService.Models.Entity;
using SP.IdentityService.Models.Request;
using SP.IdentityService.Models.Response;

namespace SP.IdentityService.Service.Impl;

/// <summary>
/// 角色服务实现类
/// </summary>
public class RoleServiceImpl : IRoleService
{
    /// <summary>
    /// 角色管理
    /// </summary>
    private readonly RoleManager<SpRole> _roleManager;

    /// <summary>
    /// 用户管理
    /// </summary>
    private readonly UserManager<SpUser> _userManager;

    /// <summary>
    /// 角色服务实现类构造函数
    /// </summary>
    /// <param name="roleManager"></param>
    /// <param name="userManager"></param>
    public RoleServiceImpl(RoleManager<SpRole> roleManager, UserManager<SpUser> userManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// 获取角色列表
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<PagedResponse<RoleResponse>> GetRoleList(RolePageRequest page)
    {
        var query = _roleManager.Roles.AsQueryable();
        if (!string.IsNullOrEmpty(page.RoleName))
        {
            query = query.Where(x => x.Name.Contains(page.RoleName));
        }

        var total = await query.CountAsync();
        var roles = await query.Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync();

        var roleResponses = roles.Select(role => new RoleResponse
        {
            Id = role.Id,
            RoleName = role.Name
        }).ToList();

        return new PagedResponse<RoleResponse>
        {
            TotalRow = total,
            TotalPage = (int)Math.Ceiling((double)total / page.PageSize),
            Data = roleResponses
        };
    }

    /// <summary>
    /// 获取角色信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<RoleResponse> GetRoleInfo(long id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null)
        {
            throw new BusinessException($"角色不存在");
        }

        return new RoleResponse
        {
            Id = role.Id,
            RoleName = role.Name
        };
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public async Task CreateRole(RoleCreateRequest role)
    {
        var spRole = new SpRole
        {
            Name = role.RoleName
        };

        var result = await _roleManager.CreateAsync(spRole);
        if (!result.Succeeded)
        {
            throw new BusinessException($"创建角色失败");
        }
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    /// <param name="id"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    public async Task UpdateRole(long id, RoleUpdateRequest role)
    {
        var spRole = await _roleManager.FindByIdAsync(id.ToString());
        if (spRole == null)
        {
            throw new BusinessException($"角色不存在");
        }

        spRole.Name = role.RoleName;

        var result = _roleManager.UpdateAsync(spRole);
        if (!result.Result.Succeeded)
        {
            throw new BusinessException($"更新角色失败");
        }
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task DeleteRole(long id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null)
        {
            throw new BusinessException($"角色不存在");
        }

        var result = _roleManager.DeleteAsync(role);
        if (!result.Result.Succeeded)
        {
            throw new BusinessException($"删除角色失败");
        }
    }
}