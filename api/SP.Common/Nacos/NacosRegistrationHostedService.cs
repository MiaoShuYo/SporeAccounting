using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SP.Common.Nacos;

/// <summary>
/// 应用启动时自动注册到 Nacos，停止时注销。
/// </summary>
public sealed class NacosRegistrationHostedService : IHostedService, IAsyncDisposable
{
    private readonly INacosClient _nacos;
    private readonly IOptions<NacosOptions> _options;
    private readonly IConfiguration _configuration;
    private readonly ILogger<NacosRegistrationHostedService> _logger;

    private string? _ip;
    private int _port;
    private Dictionary<string, string>? _metadata;
    private CancellationTokenSource? _heartbeatCts;
    private Task? _heartbeatTask;

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

        _metadata = new Dictionary<string, string>();
        var scheme = ResolveScheme();
        if (!string.IsNullOrWhiteSpace(scheme))
            _metadata["scheme"] = scheme;

        _logger.LogInformation("Registering to Nacos: {Service} {Ip}:{Port} ({Group}/{Cluster})",
            opts.ServiceName, _ip, _port, opts.GroupName, opts.ClusterName);

        await _nacos.RegisterInstanceAsync(
            serviceName: opts.ServiceName,
            ip: _ip!,
            port: _port,
            groupName: opts.GroupName,
            clusterName: opts.ClusterName,
            weight: opts.Weight,
            metadata: _metadata,
            ct: cancellationToken);

        if (opts.Ephemeral)
        {
            _heartbeatCts = new CancellationTokenSource();
            _heartbeatTask = RunHeartbeatAsync(opts, _heartbeatCts.Token);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var opts = _options.Value;
        if (!opts.RegisterEnabled) return;
        if (string.IsNullOrWhiteSpace(opts.ServiceName)) return;
        if (string.IsNullOrWhiteSpace(_ip) || _port <= 0) return;

        if (_heartbeatCts is not null)
        {
            await _heartbeatCts.CancelAsync();
            if (_heartbeatTask is not null)
            {
                try { await _heartbeatTask.WaitAsync(TimeSpan.FromSeconds(3), cancellationToken); }
                catch { /* application shutdown should continue */ }
            }
        }

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

    private async Task RunHeartbeatAsync(NacosOptions opts, CancellationToken ct)
    {
        var intervalMs = Math.Max(1_000, opts.HeartbeatIntervalMs);
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(intervalMs));

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await timer.WaitForNextTickAsync(ct);
                await _nacos.SendHeartbeatAsync(
                    serviceName: opts.ServiceName!,
                    ip: _ip!,
                    port: _port,
                    groupName: opts.GroupName,
                    clusterName: opts.ClusterName,
                    weight: opts.Weight,
                    metadata: _metadata,
                    ct: ct);
                _logger.LogDebug("Nacos heartbeat sent for {Service}", opts.ServiceName);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Nacos heartbeat failed for {Service}", opts.ServiceName);
            }
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

    public async ValueTask DisposeAsync()
    {
        if (_heartbeatCts is not null)
        {
            await _heartbeatCts.CancelAsync();
            _heartbeatCts.Dispose();
        }
    }
}
