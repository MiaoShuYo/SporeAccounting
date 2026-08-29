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

        if (skipPaths.Any(skipPath => IsSkipPathMatch(path, skipPath)))
        {
            return false; // 跳过认证的路径不需要认证
        }

        // 默认所有路径都需要认证
        return true;
    }

    private static bool IsSkipPathMatch(string path, string skipPath)
    {
        if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(skipPath))
        {
            return false;
        }

        var normalizedPath = NormalizePath(path);
        var normalizedSkipPath = NormalizePath(skipPath);

        if (normalizedSkipPath == "/")
        {
            return normalizedPath == "/";
        }

        if (normalizedPath.Equals(normalizedSkipPath, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (normalizedSkipPath.EndsWith('/'))
        {
            return normalizedPath.StartsWith(normalizedSkipPath, StringComparison.OrdinalIgnoreCase);
        }

        if (!normalizedPath.StartsWith(normalizedSkipPath, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Ensure path-segment boundary match, e.g. skip "/swagger" matches "/swagger/index.html" but not "/swagger-ui".
        return normalizedPath.Length > normalizedSkipPath.Length && normalizedPath[normalizedSkipPath.Length] == '/';
    }

    private static string NormalizePath(string value)
    {
        var trimmed = value.Trim();
        if (!trimmed.StartsWith('/'))
        {
            trimmed = "/" + trimmed;
        }

        return trimmed;
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
                throw new InvalidOperationException("Missing required configuration: Gateway:IdentityService");
            }

            var config = JsonSerializer.Deserialize<IdentityServiceConfig>(configValue);
            if (config == null
                || string.IsNullOrWhiteSpace(config.ClientId)
                || string.IsNullOrWhiteSpace(config.ClientSecret))
            {
                throw new InvalidOperationException("Invalid Gateway:IdentityService configuration: ClientId/ClientSecret must be provided");
            }
            SetCachedConfig(cacheKey, config);
            return config;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取身份服务配置时发生错误");
            throw;
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