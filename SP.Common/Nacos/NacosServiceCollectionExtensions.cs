using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SP.Common.ServiceDiscovery;

namespace SP.Common.Nacos;

public static class NacosServiceCollectionExtensions
{
    /// <summary>
    /// 注册 SP.Common 的 Nacos OpenAPI 封装。
    /// </summary>
    public static IServiceCollection AddSpNacos(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<NacosOptions>(configuration.GetSection("nacos"));

        services.AddHttpClient<INacosClient, NacosClient>((sp, http) =>
        {
            var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<NacosOptions>>().Value;
            var baseUrl = opts.ServerAddresses?.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("NacosOptions.ServerAddresses is required (nacos:ServerAddresses)");

            http.BaseAddress = new Uri(baseUrl);
            http.Timeout = TimeSpan.FromMilliseconds(opts.ConnectionTimeOut <= 0 ? 10_000 : opts.ConnectionTimeOut);
        });

        services.TryAddSingleton<IServiceDiscovery, NacosOpenApiServiceDiscovery>();
        services.AddHostedService<NacosRegistrationHostedService>();

        return services;
    }
}

