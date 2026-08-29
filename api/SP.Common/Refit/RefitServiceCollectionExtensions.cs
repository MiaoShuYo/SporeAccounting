using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Refit;
using SP.Common.ServiceDiscovery;

namespace SP.Common.Refit;

/// <summary>
/// Refit 服务注册扩展
/// </summary>
public static class RefitServiceCollectionExtensions
{
    /// <summary>
    /// 添加基于 Nacos 的 Refit 客户端。
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="serviceName">服务名称</param>
    /// <param name="groupName">分组名称</param>
    /// <param name="clusterName">集群名称</param>
    /// <param name="scheme">协议</param>
    /// <param name="refitSettings">Refit 配置</param>
    /// <typeparam name="TClient">客户端接口</typeparam>
    /// <returns>HttpClient 构建器</returns>
    public static IHttpClientBuilder AddNacosRefitClient<TClient>(
        this IServiceCollection services,
        string serviceName,
        string? groupName,
        string? clusterName,
        string scheme = "http",
        RefitSettings? refitSettings = null)
        where TClient : class
    {
        services.AddTransient<GatewaySignatureHandler>();

        return services.AddRefitClient<TClient>(refitSettings ?? new RefitSettings())
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://placeholder"))
            .AddHttpMessageHandler(sp => new NacosDiscoveryHandler(
                sp.GetRequiredService<IServiceDiscovery>(),
                serviceName,
                groupName ?? "DEFAULT_GROUP",
                clusterName ?? "DEFAULT",
                scheme,
                sp.GetRequiredService<ILogger<NacosDiscoveryHandler>>()))
            .AddHttpMessageHandler<GatewaySignatureHandler>();
    }
}