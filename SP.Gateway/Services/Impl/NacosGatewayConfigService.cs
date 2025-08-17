using System.Text.Json;
using SP.Gateway.Models;

namespace SP.Gateway.Services.Impl;

/// <summary>
/// 基于Nacos的网关配置服务实现
/// </summary>
public class NacosGatewayConfigService : IGatewayConfigService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<NacosGatewayConfigService> _logger;
    private readonly Dictionary<string, DateTime> _lastConfigCheck = new();
    private readonly Dictionary<string, object> _configCache = new();
    private readonly object _lockObject = new();
    private const int ConfigCacheMinutes = 5;

    public NacosGatewayConfigService(
        IConfiguration configuration,
        ILogger<NacosGatewayConfigService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<List<string>> GetSkipAuthenticationPathsAsync()
    {
        var cacheKey = "skip_authentication_paths";
        var cached = GetCachedConfig<List<string>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        try
        {
            var configValue = _configuration.GetSection("NoAuthPaths").Get<List<string>>();
            SetCachedConfig(cacheKey, configValue);
            return configValue ?? new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取跳过认证路径时发生错误");
            return new List<string>();
        }
    }

    public async Task<bool> IsAuthenticationRequiredAsync(string path)
    {
        var skipPaths = await GetSkipAuthenticationPathsAsync();
        if (skipPaths.Any(skipPath => path.StartsWith(skipPath, StringComparison.OrdinalIgnoreCase)))
        {
            return false; // 跳过认证的路径不需要认证
        }

        // 默认所有路径都需要认证
        return true;
    }

    public async Task<IdentityServiceConfig> GetIdentityServiceConfigAsync()
    {
        var cacheKey = "identity_service_config";
        var cached = GetCachedConfig<IdentityServiceConfig>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        try
        {
            var configValue = _configuration["Gateway:IdentityService"];
            
            if (string.IsNullOrEmpty(configValue))
            {
                var defaultConfig = new IdentityServiceConfig();
                SetCachedConfig(cacheKey, defaultConfig);
                return defaultConfig;
            }

            var config = JsonSerializer.Deserialize<IdentityServiceConfig>(configValue);
            SetCachedConfig(cacheKey, config);
            return config ?? new IdentityServiceConfig();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取身份服务配置时发生错误");
            return new IdentityServiceConfig();
        }
    }

    private T? GetCachedConfig<T>(string key)
    {
        lock (_lockObject)
        {
            if (_configCache.TryGetValue(key, out var cachedValue))
            {
                if (_lastConfigCheck.TryGetValue(key, out var lastCheck))
                {
                    var timeSinceLastCheck = DateTime.UtcNow - lastCheck;
                    if (timeSinceLastCheck.TotalMinutes < ConfigCacheMinutes)
                    {
                        return (T)cachedValue;
                    }
                }
            }
        }
        
        return default(T);
    }

    private void SetCachedConfig<T>(string key, T value)
    {
        lock (_lockObject)
        {
            _configCache[key] = value!;
            _lastConfigCheck[key] = DateTime.UtcNow;
        }
    }
}