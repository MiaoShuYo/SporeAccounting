using OpenIddict.Abstractions;
using SP.Common.ExceptionHandling.Exceptions;

namespace SP.IdentityService.Services;

/// <summary>
/// 客户端注册服务实现
/// </summary>
public class ClientRegistrationService : IClientRegistrationService
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly ILogger<ClientRegistrationService> _logger;

    public ClientRegistrationService(
        IOpenIddictApplicationManager applicationManager,
        ILogger<ClientRegistrationService> logger)
    {
        _applicationManager = applicationManager;
        _logger = logger;
    }

    /// <summary>
    /// 注册客户端
    /// </summary>
    /// <param name="clientId">客户端ID</param>
    /// <param name="clientSecret">客户端密钥</param>
    /// <param name="displayName">显示名称</param>
    /// <param name="permissions">权限</param>
    /// <returns>是否成功</returns>
    public async Task<bool> RegisterClientAsync(string clientId, string clientSecret, string displayName, string[] permissions)
    {
        try
        {
            _logger.LogInformation("开始注册客户端: {ClientId}", clientId);

            // 检查客户端是否已存在
            var existingClient = await _applicationManager.FindByClientIdAsync(clientId);
            if (existingClient != null)
            {
                _logger.LogWarning("客户端已存在: {ClientId}", clientId);
                throw new BusinessException($"客户端ID '{clientId}' 已存在");
            }

            // 创建客户端应用
            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                DisplayName = displayName,
                ClientType = OpenIddictConstants.ClientTypes.Confidential,
                Permissions =
                {
                    // 基本权限
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.Revocation,
                    
                    // 授权类型权限
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                    
                    // 范围权限
                    OpenIddictConstants.Permissions.Scopes.Email,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles,
                    
                    // 添加自定义权限
                    OpenIddictConstants.Permissions.Prefixes.Scope + "api"
                }
            };

            // 添加自定义权限
            if (permissions != null)
            {
                foreach (var permission in permissions)
                {
                    if (!string.IsNullOrEmpty(permission))
                    {
                        descriptor.Permissions.Add(permission);
                    }
                }
            }

            // 创建应用
            await _applicationManager.CreateAsync(descriptor);

            _logger.LogInformation("客户端注册成功: {ClientId}", clientId);
            return true;
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "注册客户端时发生错误: {ClientId}", clientId);
            throw new BusinessException("注册客户端失败，请稍后重试");
        }
    }

    /// <summary>
    /// 删除客户端
    /// </summary>
    /// <param name="clientId">客户端ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteClientAsync(string clientId)
    {
        try
        {
            _logger.LogInformation("开始删除客户端: {ClientId}", clientId);

            var application = await _applicationManager.FindByClientIdAsync(clientId);
            if (application == null)
            {
                _logger.LogWarning("要删除的客户端不存在: {ClientId}", clientId);
                throw new BusinessException($"客户端ID '{clientId}' 不存在");
            }

            await _applicationManager.DeleteAsync(application);

            _logger.LogInformation("客户端删除成功: {ClientId}", clientId);
            return true;
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除客户端时发生错误: {ClientId}", clientId);
            throw new BusinessException("删除客户端失败，请稍后重试");
        }
    }

    /// <summary>
    /// 获取客户端列表
    /// </summary>
    /// <returns>客户端列表</returns>
    public async Task<List<ClientInfo>> GetClientsAsync()
    {
        try
        {
            var clients = new List<ClientInfo>();
            
            await foreach (var application in _applicationManager.ListAsync())
            {
                var clientInfo = new ClientInfo
                {
                    ClientId = await _applicationManager.GetClientIdAsync(application),
                    DisplayName = await _applicationManager.GetDisplayNameAsync(application),
                    ClientType = await _applicationManager.GetClientTypeAsync(application),
                    Permissions = (await _applicationManager.GetPermissionsAsync(application)).ToList(),
                    CreatedAt = DateTime.UtcNow // OpenIddict 没有创建时间字段，使用当前时间
                };
                
                clients.Add(clientInfo);
            }

            return clients;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取客户端列表时发生错误");
            throw new BusinessException("获取客户端列表失败，请稍后重试");
        }
    }

    /// <summary>
    /// 验证客户端是否存在
    /// </summary>
    /// <param name="clientId">客户端ID</param>
    /// <returns>是否存在</returns>
    public async Task<bool> ClientExistsAsync(string clientId)
    {
        try
        {
            var application = await _applicationManager.FindByClientIdAsync(clientId);
            return application != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证客户端是否存在时发生错误: {ClientId}", clientId);
            return false;
        }
    }

    /// <summary>
    /// 初始化默认客户端（用于测试）
    /// </summary>
    /// <returns>是否成功</returns>
    public async Task<bool> InitializeDefaultClientsAsync()
    {
        try
        {
            _logger.LogInformation("开始初始化默认客户端");

            // 检查是否已存在默认客户端
            var existingClient = await _applicationManager.FindByClientIdAsync("default-client");
            if (existingClient != null)
            {
                _logger.LogInformation("默认客户端已存在，跳过初始化");
                return true;
            }

            // 创建默认客户端
            var defaultClientDescriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = "default-client",
                ClientSecret = "default-secret",
                DisplayName = "默认客户端",
                ClientType = OpenIddictConstants.ClientTypes.Confidential,
                Permissions =
                {
                    // 基本权限
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.Revocation,
                    
                    // 授权类型权限
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                    OpenIddictConstants.Permissions.GrantTypes.Password,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    
                    // 范围权限
                    OpenIddictConstants.Permissions.Scopes.Email,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles,
                    
                    // 添加自定义权限
                    OpenIddictConstants.Permissions.Prefixes.Scope + "api",
                    OpenIddictConstants.Permissions.Prefixes.Scope + "offline_access"
                }
            };

            await _applicationManager.CreateAsync(defaultClientDescriptor);

            _logger.LogInformation("默认客户端初始化成功");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "初始化默认客户端时发生错误");
            return false;
        }
    }
}
