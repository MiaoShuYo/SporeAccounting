using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SP.IdentityService.Models.Entity;

namespace SP.IdentityService.DB;

/// <summary>
/// 用户存储类
/// </summary>
public class SPUserStore : UserStore<SpUser, SpRole, IdentityServerDbContext, long>
{
    /// <summary>
    /// 用户存储类构造函数
    /// </summary>
    /// <param name="context"></param>
    /// <param name="describer"></param>
    public SPUserStore(IdentityServerDbContext context, IdentityErrorDescriber describer = null)
        : base(context, describer)
    {
    }

    /// <summary>
    /// 查找用户
    /// </summary>
    /// <param name="normalizedUserName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task<SpUser> FindByNameAsync(string normalizedUserName,
        CancellationToken cancellationToken = default)
    {
        return Users.FirstOrDefaultAsync(u => u.UserName == normalizedUserName, cancellationToken);
    }

    /// <summary>
    /// 查找用户通过邮箱
    /// </summary>
    /// <param name="normalizedEmail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task<SpUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        return Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
    }
}