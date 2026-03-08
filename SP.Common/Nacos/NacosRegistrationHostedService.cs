using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SP.Common.Nacos;

/// <summary>
/// 应用启动时自动注册到 Nacos，停止时注销。
/// </summary>
public sealed class NacosRegistrationHostedService : IHostedService
{
    private readonly INacosClient _nacos;
    private readonly IOptions<NacosOptions> _options;
    private readonly IConfiguration _configuration;
    private readonly ILogger<NacosRegistrationHostedService> _logger;

    private string? _ip;
    private int _port;

    public NacosRegistrationHostedService(
        INacosClient nacos,
        IOptions<NacosOptions> options,
        IConfiguration configuration,
        ILogger<NacosRegistrationHostedService> logger)
    {
        _nacos = nacos;
        _options = options;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var opts = _options.Value;
        if (!opts.RegisterEnabled)
        {
            _logger.LogInformation("Nacos RegisterEnabled=false, skip registration.");
            return;
        }

        if (string.IsNullOrWhiteSpace(opts.ServiceName))
        {
            _logger.LogWarning("nacos:ServiceName is empty, skip registration.");
            return;
        }

        if (!TryResolveIpPort(out _ip, out _port))
        {
            _logger.LogWarning("Cannot resolve nacos ip/port, skip registration. Set nacos:Ip and nacos:Port (or HOST_IP/EXPOSE_PORT).");
            return;
        }

        var metadata = new Dictionary<string, string>();
        var scheme = ResolveScheme();
        if (!string.IsNullOrWhiteSpace(scheme))
            metadata["scheme"] = scheme;

        _logger.LogInformation("Registering to Nacos: {Service} {Ip}:{Port} ({Group}/{Cluster})",
            opts.ServiceName, _ip, _port, opts.GroupName, opts.ClusterName);

        await _nacos.RegisterInstanceAsync(
            serviceName: opts.ServiceName,
            ip: _ip!,
            port: _port,
            groupName: opts.GroupName,
            clusterName: opts.ClusterName,
            weight: opts.Weight,
            metadata: metadata,
            ct: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var opts = _options.Value;
        if (!opts.RegisterEnabled) return;
        if (string.IsNullOrWhiteSpace(opts.ServiceName)) return;
        if (string.IsNullOrWhiteSpace(_ip) || _port <= 0) return;

        try
        {
            _logger.LogInformation("Deregistering from Nacos: {Service} {Ip}:{Port}", opts.ServiceName, _ip, _port);
            await _nacos.DeregisterInstanceAsync(
                serviceName: opts.ServiceName,
                ip: _ip,
                port: _port,
                groupName: opts.GroupName,
                clusterName: opts.ClusterName,
                ct: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Deregister from Nacos failed");
        }
    }

    private bool TryResolveIpPort(out string? ip, out int port)
    {
        ip = Environment.GetEnvironmentVariable("HOST_IP")
             ?? _configuration["HOST_IP"]
             ?? _configuration["nacos:Ip"];

        var portStr = Environment.GetEnvironmentVariable("EXPOSE_PORT")
                      ?? _configuration["EXPOSE_PORT"]
                      ?? _configuration["nacos:Port"];

        if (!string.IsNullOrWhiteSpace(ip) && int.TryParse(portStr, out port) && port > 0)
            return true;

        // fallback: try ASPNETCORE_URLS
        var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? _configuration["ASPNETCORE_URLS"];
        if (!string.IsNullOrWhiteSpace(urls))
        {
            var first = urls.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .FirstOrDefault();
            if (Uri.TryCreate(first, UriKind.Absolute, out var uri))
            {
                ip = string.IsNullOrWhiteSpace(ip) ? uri.Host : ip;
                port = uri.Port;
                return !string.IsNullOrWhiteSpace(ip) && port > 0;
            }
        }

        port = 0;
        return false;
    }

    private string? ResolveScheme()
    {
        var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? _configuration["ASPNETCORE_URLS"];
        if (string.IsNullOrWhiteSpace(urls)) return null;
        var first = urls.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault();
        return Uri.TryCreate(first, UriKind.Absolute, out var uri) ? uri.Scheme : null;
    }
}
