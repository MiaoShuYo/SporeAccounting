using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SP.Common.ExceptionHandling.Exceptions;
using SP.IdentityService.Models.Entity;
using SP.IdentityService.Models.Request;
using SP.IdentityService.Models.Response;

namespace SP.IdentityService.Service.Impl;

public class UserServiceImpl : IUserService
{
    /// <summary>
    /// 用户管理
    /// </summary>
    private readonly UserManager<SpUser> _userManager;

    /// <summary>
    /// 用户服务构造函数
    /// </summary>
    /// <param name="userManager"></param>
    public UserServiceImpl(UserManager<SpUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<UserResponse?> GetUserInfo(long id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            throw new NotFoundException($"用户不存在");
        }

        return new UserResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            IsLocked = user.LockoutEnabled
        };
    }

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<PagedResponse<UserResponse>> GetUserList(UserPageRequest page)
    {
        var query = _userManager.Users.AsQueryable();
        if (!string.IsNullOrEmpty(page.UserName))
        {
            query = query.Where(x => x.UserName.Contains(page.UserName));
        }

        if (!string.IsNullOrEmpty(page.Email))
        {
            query = query.Where(x => x.Email.Contains(page.Email));
        }

        int total = await query.CountAsync();
        List<SpUser> users = await query.OrderByDescending(o => o.Id).Skip((page.Page - 1) * page.PageSize)
            .Take(page.PageSize).ToListAsync();

        var result = new PagedResponse<UserResponse>
        {
            TotalRow = total,
            TotalPage = (int)Math.Ceiling((double)total / page.PageSize),
            Data = users.Select(x => new UserResponse
            {
                Id = x.Id,
                UserName = x.UserName,
                Email = x.Email,
                IsLocked = x.LockoutEnabled
            }).ToList()
        };

        return result;
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task DeleteUser(long id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            throw new NotFoundException($"用户不存在");
        }

        // 逻辑删除
        user.IsDeleted = true;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new BadRequestException($"删除用户失败");
        }
    }
    
    /// <summary>
    /// 禁用/启用用户
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isDisabled"></param>
    /// <returns></returns>
    public Task DisableUser(long id, bool isDisabled)
    {
        var user = _userManager.Users.FirstOrDefault(x => x.Id == id);
        if (user == null)
        {
            throw new NotFoundException($"用户不存在");
        }

        user.LockoutEnabled = isDisabled;
        var result = _userManager.UpdateAsync(user);
        if (!result.Result.Succeeded)
        {
            throw new BadRequestException($"禁用/启用用户失败");
        }

        return Task.CompletedTask;
    }
    
    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task UpdateUser(long id, UserUpdateRequest user)
    {
        var spUser = await _userManager.FindByIdAsync(id.ToString());
        if (spUser == null)
        {
            throw new NotFoundException($"用户不存在");
        }

        spUser.UserName = user.UserName;
        spUser.Email = user.Email;

        var result = await _userManager.UpdateAsync(spUser);
        if (!result.Succeeded)
        {
            throw new BadRequestException($"更新用户失败");
        }
    }
}