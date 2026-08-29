using Microsoft.Extensions.Options;
using SiNan.SDK.Registry;
using SP.Common.ServiceDiscovery;

namespace SP.Common.SiNan;

/// <summary>
/// 基于 SiNan SDK 的服务发现实现。
/// </summary>
public sealed class SiNanServiceDiscovery : IServiceDiscovery
{
    private readonly ISiNanRegistryClient _registry;
    private readonly IOptions<SiNanOptions> _options;

    public SiNanServiceDiscovery(ISiNanRegistryClient registry, IOptions<SiNanOptions> options)
    {
        _registry = registry;
        _options = options;
    }

    /// <inheritdoc />
    public async Task<Uri> ResolveAsync(
        string serviceName,
        string groupName,
        string clusterName,
        string scheme,
        CancellationToken ct = default)
    {
        var instances = await GetHealthyInstancesAsync(serviceName, groupName, ct);

        if (instances.Count == 0)
            throw new InvalidOperationException(
                $"No healthy instance found for service '{serviceName}' in group '{groupName}'.");

        // 随机负载均衡，避免所有请求打到同一实例
        var instance = instances[Random.Shared.Next(instances.Count)];
        return BuildUri(scheme, instance);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Uri>> ListAsync(
        string serviceName,
        string groupName,
        string clusterName,
        string scheme,
        CancellationToken ct = default)
    {
        var instances = await GetHealthyInstancesAsync(serviceName, groupName, ct);
        return instances.Select(i => BuildUri(scheme, i)).ToList();
    }

    private async Task<IReadOnlyList<ServiceInstance>> GetHealthyInstancesAsync(
        string serviceName,
        string groupName,
        CancellationToken ct)
    {
        var opts = _options.Value;
        var ns = opts.Namespace;
        var group = string.IsNullOrWhiteSpace(groupName) ? opts.Group : groupName;

        var result = await _registry.GetInstancesAsync(ns, group, serviceName, healthyOnly: true, cancellationToken: ct);
        return result.Value?.Instances ?? [];
    }

    private static Uri BuildUri(string scheme, ServiceInstance instance)
    {
        var finalScheme = string.IsNullOrWhiteSpace(scheme) ? "http" : scheme;
        return new Uri($"{finalScheme}://{instance.Host}:{instance.Port}");
    }
}
