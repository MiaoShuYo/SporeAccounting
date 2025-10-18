using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SP.Common;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Model;
using SP.Common.Redis;
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
    /// Redis服务
    /// </summary>
    private readonly IRedisService _redis;

    /// <summary>
    /// 上下文会话
    /// </summary>
    private readonly ContextSession _contextSession;

    /// <summary>
    /// 用户服务构造函数
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="redis"></param>
    /// <param name="contextSession"></param>
    public UserServiceImpl(UserManager<SpUser> userManager, IRedisService redis, ContextSession contextSession)
    {
        _redis = redis;
        _userManager = userManager;
        _contextSession = contextSession;
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<UserResponse?> GetUserInfo(long id)
    {
        // 尝试从缓存获取
        string cacheKey = $"user:{id}";
        var cachedUser = await _redis.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedUser))
        {
            return JsonSerializer.Deserialize<UserResponse>(cachedUser);
        }

        // 缓存未命中，从数据库查询
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            throw new NotFoundException($"用户不存在");
        }

        var response = new UserResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            IsLocked = user.LockoutEnabled
        };
        // 缓存结果，设置适当的过期时间
        await _redis.SetStringAsync(cacheKey, JsonSerializer.Serialize(response), 60 * 10);
        return response;
    }

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<PageResponse<UserResponse>> GetUserList(UserPageRequest page)
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

        // 使用单次查询获取总数和分页数据
        var totalQuery = query;
        var pagedQuery = query.OrderByDescending(o => o.Id)
            .Skip((page.Page - 1) * page.PageSize)
            .Take(page.PageSize);

        // 并行执行两个查询
        var countTask = totalQuery.CountAsync();
        var usersTask = pagedQuery.ToListAsync();

        await Task.WhenAll(countTask, usersTask);

        int total = countTask.Result;
        List<SpUser> users = usersTask.Result;

        var result = new PageResponse<UserResponse>
        {
            TotalCount = total,
            TotalPage = (int)Math.Ceiling((double)total / page.PageSize),
            Data = users.Select(x => new UserResponse
            {
                Id = x.Id,
                UserName = x.UserName,
                Email = x.Email,
                IsLocked = x.LockoutEnabled,
                PhoneNumber = x.PhoneNumber
            }).ToList(),
            PageSize= page.PageSize,
            PageIndex= page.Page
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
    public async Task DisableUser(long id, bool isDisabled)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            throw new NotFoundException($"用户不存在");
        }

        if (isDisabled)
        {
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100); // 设置一个很远的未来时间
        }
        else
        {
            user.LockoutEnd = null; // 解除锁定
        }

        user.LockoutEnabled = isDisabled;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new BadRequestException($"禁用/启用用户失败");
        }
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
        spUser.PhoneNumber = user.PhoneNumber;

        var result = await _userManager.UpdateAsync(spUser);
        if (!result.Succeeded)
        {
            throw new BadRequestException($"更新用户失败");
        }
    }

    /// <summary>
    /// 查询用户手机是否已验证
    /// </summary>
    /// <returns></returns>
    public async Task<bool> IsUserPhoneVerified()
    {
        var userId = _contextSession.UserId;
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            throw new NotFoundException("用户不存在");
        }

        return user.PhoneNumberConfirmed;
    }

    /// <summary>
    /// 查询用户邮箱是否已验证
    /// </summary>
    /// <returns></returns>
    public async Task<bool> IsUserEmailVerified()
    {
        var userId = _contextSession.UserId;
        var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            throw new NotFoundException("用户不存在");
        }

        return user.EmailConfirmed;
    }
}