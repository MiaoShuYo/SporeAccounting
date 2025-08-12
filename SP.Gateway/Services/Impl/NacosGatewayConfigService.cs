using System.Text.Json;
using Nacos.V2;
using SP.Gateway.Models;

namespace SP.Gateway.Services.Impl;

/// <summary>
/// 基于Nacos的网关配置服务实现
/// </summary>
public class NacosGatewayConfigService : IGatewayConfigService
{
    private readonly INacosNamingService _namingService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<NacosGatewayConfigService> _logger;
    private readonly Dictionary<string, DateTime> _lastConfigCheck = new();
    private readonly Dictionary<string, object> _configCache = new();
    private readonly object _lockObject = new();
    private const string ConfigServiceName = "SPConfigService";
    private const int ConfigCacheMinutes = 5;

    public NacosGatewayConfigService(
        INacosNamingService namingService,
        IConfiguration configuration,
        ILogger<NacosGatewayConfigService> logger)
    {
        _namingService = namingService;
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
            var configValue = await GetConfigFromNacosAsync("Gateway:SkipAuthenticationPaths");
            
            if (string.IsNullOrEmpty(configValue))
            {
                var defaultPaths = new List<string>
                {
                    "/swagger",
                    "/api/auth",
                    "/health",
                    "/favicon.ico",
                    "/.well-known"
                };
                
                SetCachedConfig(cacheKey, defaultPaths);
                return defaultPaths;
            }

            var paths = JsonSerializer.Deserialize<List<string>>(configValue);
            SetCachedConfig(cacheKey, paths);
            return paths ?? new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取跳过认证路径时发生错误");
            return new List<string>();
        }
    }

    public async Task<List<string>> GetRequireAuthenticationPathsAsync()
    {
        var cacheKey = "require_authentication_paths";
        var cached = GetCachedConfig<List<string>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        try
        {
            var configValue = await GetConfigFromNacosAsync("Gateway:RequireAuthenticationPaths");
            
            if (string.IsNullOrEmpty(configValue))
            {
                var defaultPaths = new List<string>
                {
                    "/api/finance",
                    "/api/currency",
                    "/api/config",
                    "/api/report"
                };
                
                SetCachedConfig(cacheKey, defaultPaths);
                return defaultPaths;
            }

            var paths = JsonSerializer.Deserialize<List<string>>(configValue);
            SetCachedConfig(cacheKey, paths);
            return paths ?? new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取需要认证路径时发生错误");
            return new List<string>();
        }
    }

    public async Task<bool> IsAuthenticationRequiredAsync(string path)
    {
        var skipPaths = await GetSkipAuthenticationPathsAsync();
        if (skipPaths.Any(skipPath => path.StartsWith(skipPath, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var requirePaths = await GetRequireAuthenticationPathsAsync();
        if (requirePaths.Any(requirePath => path.StartsWith(requirePath, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

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
            var configValue = await GetConfigFromNacosAsync("Gateway:IdentityService");
            
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

    private async Task<string?> GetConfigFromNacosAsync(string configKey)
    {
        try
        {
            var configServiceUrl = await GetConfigServiceUrlAsync();
            if (!string.IsNullOrEmpty(configServiceUrl))
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"{configServiceUrl}/api/config/{configKey}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
            
            return _configuration[configKey];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "从Nacos获取配置 {ConfigKey} 时发生错误", configKey);
            return null;
        }
    }

    private async Task<string?> GetConfigServiceUrlAsync()
    {
        try
        {
            var instances = await _namingService.SelectInstances(ConfigServiceName, "DEFAULT_GROUP", new List<string> { "DEFAULT" }, true);
            if (instances?.Any() == true)
            {
                var instance = instances.First(h => h.Enabled && h.Healthy);
                var scheme = instance.Metadata?.GetValueOrDefault("scheme", "http");
                return $"{scheme}://{instance.Ip}:{instance.Port}";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取配置服务URL时发生错误");
        }
        
        return null;
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