using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;

namespace SP.IdentityService.DB;

/// <summary>
/// 种子数据
/// </summary>
public class SeedData: IHostedService
{
    private readonly UserManager<IdentityUser> _userManager;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userManager"></param>
    public SeedData(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }
    
    /// <summary>
    /// 启动时执行
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // 设置管理员账号
        var adminUser = new IdentityUser
        {
            UserName = "admin",
            Email = "admin@SporeAccounting.com",
            EmailConfirmed = true,
        };
        await _userManager.CreateAsync(adminUser, "123*asdasd");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
