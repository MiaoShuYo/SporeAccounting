using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SiNan.SDK.Registry;

namespace SP.Common.SiNan;

/// <summary>
/// 应用启动时自动注册到 SiNan，并周期性发送心跳；停止时注销实例。
/// </summary>
public sealed class SiNanRegistrationHostedService : IHostedService, IAsyncDisposable
{
    private readonly ISiNanRegistryClient _registry;
    private readonly IOptions<SiNanOptions> _options;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SiNanRegistrationHostedService> _logger;

    private string? _ip;
    private int _port;
    private string? _instanceId;
    private CancellationTokenSource? _heartbeatCts;
    private Task? _heartbeatTask;

    public SiNanRegistrationHostedService(
        ISiNanRegistryClient registry,
        IOptions<SiNanOptions> options,
        IConfiguration configuration,
        ILogger<SiNanRegistrationHostedService> logger)
    {
        _registry = registry;
        _options = options;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var opts = _options.Value;
        if (!opts.RegisterEnabled)
        {
            _logger.LogInformation("SiNan RegisterEnabled=false, skip registration.");
            return;
        }

        if (string.IsNullOrWhiteSpace(opts.ServiceName))
        {
            _logger.LogWarning("sinan:ServiceName is empty, skip registration.");
            return;
        }

        if (!TryResolveIpPort(out _ip, out _port))
        {
            _logger.LogWarning(
                "Cannot resolve ip/port for SiNan registration. " +
                "Set HOST_IP and EXPOSE_PORT environment variables (or ASPNETCORE_URLS).");
            return;
        }

        _logger.LogInformation(
            "Registering to SiNan: {Service} {Ip}:{Port} ({Namespace}/{Group})",
            opts.ServiceName, _ip, _port, opts.Namespace, opts.Group);

        var result = await _registry.RegisterAsync(new RegisterInstanceRequest
        {
            Namespace = opts.Namespace,
            Group = opts.Group,
            ServiceName = opts.ServiceName,
            Host = _ip!,
            Port = _port,
            Weight = opts.Weight,
            TtlSeconds = opts.TtlSeconds,
            IsEphemeral = true
        }, cancellationToken);

        _instanceId = result.InstanceId;
        _logger.LogInformation("SiNan registration succeeded. InstanceId={InstanceId}", _instanceId);

        // 启动心跳后台任务
        _heartbeatCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _heartbeatTask = RunHeartbeatAsync(opts, _heartbeatCts.Token);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var opts = _options.Value;
        if (!opts.RegisterEnabled) return;
        if (string.IsNullOrWhiteSpace(opts.ServiceName)) return;
        if (string.IsNullOrWhiteSpace(_ip) || _port <= 0) return;

        // 停止心跳
        if (_heartbeatCts != null)
        {
            await _heartbeatCts.CancelAsync();
            if (_heartbeatTask != null)
            {
                try { await _heartbeatTask.WaitAsync(TimeSpan.FromSeconds(3), cancellationToken); }
                catch { /* ignore */ }
            }
        }

        try
        {
            _logger.LogInformation(
                "Deregistering from SiNan: {Service} {Ip}:{Port}", opts.ServiceName, _ip, _port);

            await _registry.DeregisterAsync(new DeregisterInstanceRequest
            {
                Namespace = opts.Namespace,
                Group = opts.Group,
                ServiceName = opts.ServiceName,
                Host = _ip!,
                Port = _port
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Deregister from SiNan failed (will be removed by TTL expiry).");
        }
    }

    private async Task RunHeartbeatAsync(SiNanOptions opts, CancellationToken ct)
    {
        var interval = TimeSpan.FromSeconds(opts.ResolvedHeartbeatIntervalSeconds);
        using var timer = new PeriodicTimer(interval);

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await timer.WaitForNextTickAsync(ct);

                await _registry.HeartbeatAsync(new HeartbeatRequest
                {
                    Namespace = opts.Namespace,
                    Group = opts.Group,
                    ServiceName = opts.ServiceName!,
                    Host = _ip!,
                    Port = _port
                }, ct);

                _logger.LogDebug("SiNan heartbeat sent for {Service}", opts.ServiceName);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SiNan heartbeat failed for {Service}", opts.ServiceName);
            }
        }
    }

    private bool TryResolveIpPort(out string? ip, out int port)
    {
        ip = Environment.GetEnvironmentVariable("HOST_IP")
             ?? _configuration["HOST_IP"]
             ?? _configuration["sinan:Ip"];

        var portStr = Environment.GetEnvironmentVariable("EXPOSE_PORT")
                      ?? _configuration["EXPOSE_PORT"]
                      ?? _configuration["sinan:Port"];

        if (!string.IsNullOrWhiteSpace(ip) && int.TryParse(portStr, out port) && port > 0)
            return true;

        // fallback: 从 ASPNETCORE_URLS 推断
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

    public async ValueTask DisposeAsync()
    {
        if (_heartbeatCts != null)
        {
            await _heartbeatCts.CancelAsync();
            _heartbeatCts.Dispose();
        }
    }
}
