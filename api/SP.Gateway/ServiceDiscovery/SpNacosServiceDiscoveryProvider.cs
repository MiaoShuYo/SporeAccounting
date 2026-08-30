using Microsoft.Extensions.Options;
using Ocelot.Configuration;
using Ocelot.ServiceDiscovery.Providers;
using Ocelot.Values;
using SP.Common.Nacos;

namespace SP.Gateway.ServiceDiscovery;

public sealed class SpNacosServiceDiscoveryProvider : IServiceDiscoveryProvider
{
    private readonly INacosClient _nacos;
    private readonly IOptions<NacosOptions> _nacosOptions;
    private readonly DownstreamRoute _route;

    public SpNacosServiceDiscoveryProvider(INacosClient nacos, IOptions<NacosOptions> nacosOptions, DownstreamRoute route)
    {
        _nacos = nacos;
        _nacosOptions = nacosOptions;
        _route = route;
    }

    public async Task<List<Service>> GetAsync()
    {
        var serviceName = _route.ServiceName;
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            return new List<Service>();
        }

        // Ocelot 的 ServiceNamespace 这里按 Nacos GroupName 处理；未配置时回退到 nacos:GroupName
        var groupName = string.IsNullOrWhiteSpace(_route.ServiceNamespace)
            ? _nacosOptions.Value.GroupName
            : _route.ServiceNamespace;

        var clusterName = _nacosOptions.Value.ClusterName;

        var instances = await _nacos.ListHealthyInstancesAsync(serviceName, groupName, clusterName, CancellationToken.None);

        var scheme = string.IsNullOrWhiteSpace(_route.DownstreamScheme) ? "http" : _route.DownstreamScheme;

        var services = new List<Service>(instances.Count);
        foreach (var ins in instances)
        {
            if (!ins.Healthy || !ins.Enabled) continue;

            var hostAndPort = new ServiceHostAndPort(ins.Ip, ins.Port, scheme);
            services.Add(new Service(
                name: serviceName,
                hostAndPort: hostAndPort,
                id: $"{ins.Ip}:{ins.Port}",
                version: string.Empty,
                tags: Array.Empty<string>()));
        }

        return services;
    }
}
