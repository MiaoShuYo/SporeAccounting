using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SP.Common.ServiceDiscovery;

namespace SP.Common.Nacos;

public static class NacosServiceCollectionExtensions
{
    /// <summary>
    /// 注册 SP.Common 的 Nacos OpenAPI 封装。
    /// 生产环境建议通过 NACOS_USERNAME / NACOS_PASSWORD 环境变量覆盖凭证，
    /// 避免将明文密码提交到源代码仓库。
    /// </summary>
    public static IServiceCollection AddSpNacos(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<NacosOptions>(configuration.GetSection("nacos"));

        // 支持通过环境变量覆盖 Nacos 鉴权凭证（优先级高于 appsettings.json）
        services.PostConfigure<NacosOptions>(opts =>
        {
            var envUsername = Environment.GetEnvironmentVariable("NACOS_USERNAME");
            var envPassword = Environment.GetEnvironmentVariable("NACOS_PASSWORD");
            if (!string.IsNullOrWhiteSpace(envUsername))
                opts.Username = envUsername;
            if (!string.IsNullOrWhiteSpace(envPassword))
                opts.Password = envPassword;
        });

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

