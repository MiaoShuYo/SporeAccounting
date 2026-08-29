using SP.Common.ServiceDiscovery;

namespace SP.Common.Nacos;

/// <summary>
/// 基于 <see cref="INacosClient"/>（OpenAPI）的服务发现实现。
/// </summary>
public sealed class NacosOpenApiServiceDiscovery : IServiceDiscovery
{
    private readonly INacosClient _nacos;

    public NacosOpenApiServiceDiscovery(INacosClient nacos) => _nacos = nacos;

    public Task<Uri> ResolveAsync(string serviceName, string groupName, string clusterName, string scheme,
        CancellationToken ct = default)
        => _nacos.ResolveAsync(serviceName, groupName, clusterName, scheme, ct);

    public async Task<IReadOnlyList<Uri>> ListAsync(string serviceName, string groupName, string clusterName,
        string scheme, CancellationToken ct = default)
    {
        var instances = await _nacos.ListHealthyInstancesAsync(serviceName, groupName, clusterName, ct);
        var uris = new List<Uri>(instances.Count);
        foreach (var ins in instances)
        {
            var finalScheme = ins.Metadata?.GetValueOrDefault("scheme", scheme) ?? scheme;
            uris.Add(new Uri($"{finalScheme}://{ins.Ip}:{ins.Port}"));
        }
        return uris;
    }
}
