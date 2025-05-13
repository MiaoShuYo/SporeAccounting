using OpenIddict.Abstractions;

namespace SP.IdentityService.DB;

/// <summary>
/// 种子数据
/// </summary>
public class SeedData: IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serviceProvider"></param>
    public SeedData(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    /// <summary>
    /// 启动时执行
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        
        // 这是管理员Admin的客户端id
        string clientId = "8E60948B-A27C-4335-AFC3-400C25E7CC2E";
        // 检查是否存在指定的 client_id
        if (await applicationManager.FindByClientIdAsync(clientId, cancellationToken) is null)
        {
            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                ClientSecret = "ZxOfk4T4ncFAjcGZIUvIKSszxXy7qFoN", // 设置密钥
                DisplayName = "Admin管理员客户端",
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.Password,           // 允许密码模式
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                    OpenIddictConstants.Permissions.Prefixes.Scope + "api"          // 允许访问的 API 作用域
                }
            };

            await applicationManager.CreateAsync(descriptor, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
