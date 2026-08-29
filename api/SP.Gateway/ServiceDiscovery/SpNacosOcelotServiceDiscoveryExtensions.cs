using Microsoft.Extensions.DependencyInjection;
using Ocelot.ServiceDiscovery;

namespace SP.Gateway.ServiceDiscovery;

public static class SpNacosOcelotServiceDiscoveryExtensions
{
    public static IServiceCollection AddSpNacosServiceDiscoveryForOcelot(this IServiceCollection services)
    {
        // Ocelot 默认也是单例工厂，这里直接覆盖即可。
        services.AddSingleton<IServiceDiscoveryProviderFactory, SpNacosServiceDiscoveryProviderFactory>();
        return services;
    }
}
