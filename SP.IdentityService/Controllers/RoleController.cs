using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SP.Common;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Model;
using SP.IdentityService.Models.Request;
using SP.IdentityService.Models.Response;
using SP.IdentityService.Service;

namespace SP.IdentityService.Controllers;

/// <summary>
/// 角色控制器
/// </summary>
[ApiController]
[Route("/api/roles")]
[Authorize]
public class RoleController : ControllerBase
{
    /// <summary>
    /// 角色服务
    /// </summary>
    private readonly IRoleService _roleService;

    /// <summary>
    /// 角色权限服务
    /// </summary>
    private readonly IRolePermissionService _rolePermissionService;

    /// <summary>
    /// 上下文会话
    /// </summary>
    private readonly ContextSession _contextSession;

    /// <summary>
    /// 角色控制器构造函数
    /// </summary>
    /// <param name="roleService"></param>
    /// <param name="rolePermissionService"></param>
    /// <param name="contextSession"></param>
    public RoleController(IRoleService roleService, IRolePermissionService rolePermissionService, ContextSession contextSession)
    {
        _rolePermissionService = rolePermissionService;
        _roleService = roleService;
        _contextSession = contextSession;
    }

    /// <summary>
    /// 获取角色列表
    /// </summary>
    /// <param name="page">分页查询参数</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<PageResponse<RoleResponse>>> GetRoles([FromQuery] RolePageRequest page)
    {
        var result = await _roleService.GetRoleList(page);
        return Ok(result);
    }

    /// <summary>
    /// 获取角色信息
    /// </summary>
    /// <param name="id">角色id</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<RoleResponse>> GetRole([FromRoute] long id)
    {
        var result = await _roleService.GetRoleInfo(id);
        return Ok(result);
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="role">角色信息</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> CreateRole([FromBody] RoleCreateRequest role)
    {
        await _roleService.CreateRole(role);
        return Ok();
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    /// <param name="id">角色id</param>
    /// <param name="role">角色信息</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateRole([FromRoute] long id, [FromBody] RoleUpdateRequest role)
    {
        await _roleService.UpdateRole(id, role);
        return Ok();
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="id">角色id</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRole([FromRoute] long id)
    {
        await _roleService.DeleteRole(id);
        return Ok();
    }

    /// <summary>
    /// 为角色添加权限
    /// </summary>
    /// <param name="roleId">角色id</param>
    /// <param name="permission">权限</param>
    [HttpPost("{roleId}/permissions")]
    public async Task<ActionResult> AddPermissionToRole([FromRoute] long roleId, [FromBody] string permission)
    {
        await _rolePermissionService.AddPermissionToRole(roleId, permission);
        return Ok();
    }

    /// <summary>
    /// 获取角色权限
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    [HttpGet("{roleId}/permissions")]
    public async Task<ActionResult<List<string>>> GetRolePermissions([FromRoute] long roleId)
    {
        var result = await _rolePermissionService.GetPermissionsByRole(roleId);
        return Ok(result);
    }

    /// <summary>
    /// 删除角色权限
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="permission"></param>
    [HttpDelete("{roleId}/permissions/{permission}")]
    public async Task<ActionResult> RemovePermissionFromRole([FromRoute] long roleId, [FromRoute] string permission)
    {
        await _rolePermissionService.RemovePermissionFromRole(roleId, permission);
        return Ok();
    }

    /// <summary>
    /// 获取用户权限（只能查询自身的权限）
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("users/{userId}/permissions")]
    public async Task<ActionResult<List<string>>> GetUserPermissions([FromRoute] long userId)
    {
        if (userId != _contextSession.UserId)
        {
            throw new ForbiddenException("不能查看其他用户的权限");
        }

        var result = await _rolePermissionService.GetUserPermissions(userId);
        return Ok(result);
    }
}