using Nacos.V2;

namespace SP.Gateway.Services.Impl;

/// <summary>
/// Nacos服务发现服务实现
/// </summary>
public class NacosServiceDiscoveryService : INacosServiceDiscoveryService
{
    private readonly INacosNamingService _namingService;
    private readonly HttpClient _httpClient;
    private readonly ILogger<NacosServiceDiscoveryService> _logger;
    private readonly Dictionary<string, DateTime> _lastHealthCheck = new();
    private readonly Dictionary<string, bool> _healthStatus = new();
    private readonly object _lockObject = new();
    private const string IdentityServiceName = "SPIdentityService";

    public NacosServiceDiscoveryService(
        INacosNamingService namingService,
        HttpClient httpClient,
        ILogger<NacosServiceDiscoveryService> logger)
    {
        _namingService = namingService;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<string>> GetIdentityServiceUrlsAsync()
    {
        try
        {
            _logger.LogInformation("开始从Nacos获取身份服务实例，服务名: {ServiceName}", IdentityServiceName);
            
            var instances = await _namingService.SelectInstances(IdentityServiceName, "DEFAULT_GROUP", new List<string> { "DEFAULT" }, true);
            
            _logger.LogInformation("从Nacos获取到 {Count} 个实例", instances?.Count ?? 0);
            
            if (instances == null || !instances.Any())
            {
                _logger.LogWarning("从Nacos获取身份服务实例失败或没有可用实例");
                return new List<string>();
            }

            var urls = new List<string>();
            foreach (var instance in instances)
            {
                _logger.LogDebug("检查实例: IP={Ip}, Port={Port}, Enabled={Enabled}, Healthy={Healthy}", 
                    instance.Ip, instance.Port, instance.Enabled, instance.Healthy);
                
                if (instance.Enabled && instance.Healthy)
                {
                    var scheme = instance.Metadata?.GetValueOrDefault("scheme", "http");
                    var url = $"{scheme}://{instance.Ip}:{instance.Port}";
                    urls.Add(url);
                    _logger.LogDebug("添加健康实例: {Url}", url);
                }
                else
                {
                    _logger.LogDebug("跳过不健康实例: {Ip}:{Port}", instance.Ip, instance.Port);
                }
            }

            _logger.LogInformation("从Nacos获取到 {Count} 个身份服务实例", urls.Count);
            return urls;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "从Nacos获取身份服务实例时发生错误");
            return new List<string>();
        }
    }

    public async Task<string?> GetBestIdentityServiceUrlAsync()
    {
        _logger.LogInformation("开始获取最佳身份服务URL");
        
        var urls = await GetIdentityServiceUrlsAsync();
        
        if (!urls.Any())
        {
            _logger.LogWarning("没有可用的身份服务实例");
            return null;
        }

        var availableUrls = new List<string>();
        foreach (var url in urls)
        {
            _logger.LogDebug("检查服务可用性: {Url}", url);
            if (await IsServiceAvailableAsync(url))
            {
                availableUrls.Add(url);
                _logger.LogDebug("服务可用: {Url}", url);
            }
            else
            {
                _logger.LogDebug("服务不可用: {Url}", url);
            }
        }

        if (!availableUrls.Any())
        {
            _logger.LogWarning("所有身份服务实例都不可用");
            return null;
        }

        var random = new Random();
        var selectedUrl = availableUrls[random.Next(availableUrls.Count)];
        
        _logger.LogInformation("选择身份服务URL: {Url}", selectedUrl);
        return selectedUrl;
    }

    public async Task<bool> IsServiceAvailableAsync(string url)
    {
        lock (_lockObject)
        {
            if (_lastHealthCheck.TryGetValue(url, out var lastCheck))
            {
                var timeSinceLastCheck = DateTime.UtcNow - lastCheck;
                if (timeSinceLastCheck.TotalSeconds < 5)
                {
                    var cachedStatus = _healthStatus.GetValueOrDefault(url, false);
                    _logger.LogDebug("使用缓存的健康状态: {Url} = {Status}", url, cachedStatus);
                    return cachedStatus;
                }
            }
        }

        try
        {
            var discoveryUrl = $"{url.TrimEnd('/')}/.well-known/openid_configuration";
            _logger.LogDebug("检查健康状态: {Url}", discoveryUrl);
            
            var response = await _httpClient.GetAsync(discoveryUrl, CancellationToken.None);
            
            var isHealthy = response.IsSuccessStatusCode;
            
            lock (_lockObject)
            {
                _healthStatus[url] = isHealthy;
                _lastHealthCheck[url] = DateTime.UtcNow;
            }
            
            if (isHealthy)
            {
                _logger.LogDebug("身份服务 {Url} 健康检查通过", url);
            }
            else
            {
                _logger.LogWarning("身份服务 {Url} 健康检查失败，状态码: {StatusCode}", url, response.StatusCode);
            }
            
            return isHealthy;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查身份服务 {Url} 健康状态时发生错误", url);
            
            lock (_lockObject)
            {
                _healthStatus[url] = false;
                _lastHealthCheck[url] = DateTime.UtcNow;
            }
            
            return false;
        }
    }
}