using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ocelot.ServiceDiscovery;
using SP.Common.Nacos;

namespace SP.Gateway.ServiceDiscovery;

public static class SpNacosOcelotServiceDiscoveryExtensions
{
    public static IServiceCollection AddSpNacosServiceDiscoveryForOcelot(this IServiceCollection services)
    {
        // 保留 Provider Factory，兼容旧版 Ocelot 的服务发现扩展点。
        services.AddSingleton<IServiceDiscoveryProviderFactory, SpNacosServiceDiscoveryProviderFactory>();

        // Ocelot 25 在启动校验和运行时通过 Finder Delegate 解析 Provider。
        // 仅注册 Factory 会导致所有含 ServiceName 的路由在启动阶段校验失败。
        services.AddSingleton<ServiceDiscoveryFinderDelegate>(serviceProvider =>
        {
            var nacos = serviceProvider.GetRequiredService<INacosClient>();
            var nacosOptions = serviceProvider.GetRequiredService<IOptions<NacosOptions>>();

            return (_, _, route) => new SpNacosServiceDiscoveryProvider(nacos, nacosOptions, route);
        });

        return services;
    }
}
