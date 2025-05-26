using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SP.IdentityService.Models.Entity;

namespace SP.IdentityService.Impl;

public class RolePermissionService : IRolePermissionService
{
    private readonly RoleManager<SpRole> _roleManager;
    private readonly UserManager<SpUser> _userManager;

    public RolePermissionService(RoleManager<SpRole> roleManager, UserManager<SpUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task AddPermissionToRole(long roleId, string permission)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null) throw new Exception("角色不存在");

        var claim = new Claim("Permission", permission);
        var result = await _roleManager.AddClaimAsync(role, claim);
        if (!result.Succeeded) throw new Exception("添加权限失败");
    }

    public async Task<List<string>> GetPermissionsByRole(long roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null) throw new Exception("角色不存在");

        var claims = await _roleManager.GetClaimsAsync(role);
        return claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();
    }

    public async Task RemovePermissionFromRole(long roleId, string permission)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null) throw new Exception("角色不存在");

        var claim = new Claim("Permission", permission);
        var result = await _roleManager.RemoveClaimAsync(role, claim);
        if (!result.Succeeded) throw new Exception("删除权限失败");
    }

    public async Task<List<string>> GetUserPermissions(long userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new Exception("用户不存在");

        var roles = await _userManager.GetRolesAsync(user);
        var permissions = new List<string>();

        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var claims = await _roleManager.GetClaimsAsync(role);
                permissions.AddRange(claims.Where(c => c.Type == "Permission").Select(c => c.Value));
            }
        }

        return permissions.Distinct().ToList();
    }
}