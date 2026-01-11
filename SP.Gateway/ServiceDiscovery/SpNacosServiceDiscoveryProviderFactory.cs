using Microsoft.Extensions.Options;
using Ocelot.Configuration;
using Ocelot.Responses;
using Ocelot.ServiceDiscovery;
using Ocelot.ServiceDiscovery.Providers;
using SP.Common.Nacos;

namespace SP.Gateway.ServiceDiscovery;

public sealed class SpNacosServiceDiscoveryProviderFactory : IServiceDiscoveryProviderFactory
{
    private readonly INacosClient _nacos;
    private readonly IOptions<NacosOptions> _nacosOptions;

    public SpNacosServiceDiscoveryProviderFactory(INacosClient nacos, IOptions<NacosOptions> nacosOptions)
    {
        _nacos = nacos;
        _nacosOptions = nacosOptions;
    }

    public Response<IServiceDiscoveryProvider> Get(ServiceProviderConfiguration serviceConfig, DownstreamRoute route)
    {
        // 仅接管 Type=nacos 的场景（兼容 Ocelot.Provider.Nacos 的配置习惯）
        if (!string.Equals(serviceConfig?.Type, "nacos", StringComparison.OrdinalIgnoreCase))
        {
            return new OkResponse<IServiceDiscoveryProvider>(new NoopServiceDiscoveryProvider());
        }

        return new OkResponse<IServiceDiscoveryProvider>(new SpNacosServiceDiscoveryProvider(_nacos, _nacosOptions, route));
    }

    private sealed class NoopServiceDiscoveryProvider : IServiceDiscoveryProvider
    {
        public Task<List<Ocelot.Values.Service>> GetAsync() => Task.FromResult(new List<Ocelot.Values.Service>());
    }
}
