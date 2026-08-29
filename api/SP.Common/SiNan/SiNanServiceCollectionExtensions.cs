using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SiNan.SDK;
using SP.Common.ServiceDiscovery;

namespace SP.Common.SiNan;

public static class SiNanServiceCollectionExtensions
{
    /// <summary>
    /// 注册 SP.Common 的 SiNan 封装：服务注册/发现 + SDK 客户端。
    /// 生产环境建议通过环境变量 SINAN_APIKEY 覆盖 ApiKey，避免明文提交到仓库。
    /// </summary>
    public static IServiceCollection AddSpSiNan(this IServiceCollection services, IConfiguration configuration)
    {
        // 绑定配置
        services.Configure<SiNanOptions>(configuration.GetSection("sinan"));

        // 支持通过环境变量覆盖 ApiKey
        services.PostConfigure<SiNanOptions>(opts =>
        {
            var envApiKey = Environment.GetEnvironmentVariable("SINAN_APIKEY");
            if (!string.IsNullOrWhiteSpace(envApiKey))
                opts.ApiKey = envApiKey;
        });

        // 注册 SiNan SDK 客户端（ISiNanRegistryClient + ISiNanConfigClient）
        services.AddSiNanClients(sdkOpts =>
        {
            // 在 PostConfigure 执行之后读取最终选项，因此通过工厂方式延迟解析
        });

        // 用真实 SiNanOptions 覆盖 SDK 选项，并在需要时注入 ApiKey 到 HttpClient
        services.PostConfigureAll<SiNanClientOptions>(sdkOpts =>
        {
            // 此处无法安全拿到 SiNanOptions，改用 IConfigureOptions 方式（见下方）
        });

        // 通过 IConfigureOptions 将 SiNanOptions 映射到 SiNanClientOptions
        services.AddSingleton<IConfigureOptions<SiNanClientOptions>>(sp =>
        {
            var sinanOpts = sp.GetRequiredService<IOptions<SiNanOptions>>().Value;
            return new ConfigureOptions<SiNanClientOptions>(sdkOpts =>
            {
                sdkOpts.BaseUrl = sinanOpts.BaseUrl;
                sdkOpts.Timeout = TimeSpan.FromMilliseconds(
                    sinanOpts.ConnectionTimeOut <= 0 ? 10_000 : sinanOpts.ConnectionTimeOut);
                sdkOpts.RetryCount = sinanOpts.RetryCount;
                sdkOpts.RetryDelayMs = sinanOpts.RetryDelayMs;
                sdkOpts.RetryMaxDelayMs = sinanOpts.RetryMaxDelayMs;
            });
        });

        // 若配置了 ApiKey，追加 X-SiNan-Token 请求头到 "SiNan" HttpClient
        services.AddHttpClient("SiNan")
            .ConfigureHttpClient((sp, client) =>
            {
                var sinanOpts = sp.GetRequiredService<IOptions<SiNanOptions>>().Value;
                if (!string.IsNullOrWhiteSpace(sinanOpts.ApiKey))
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(
                        "X-SiNan-Token", sinanOpts.ApiKey);
                }
            });

        services.TryAddSingleton<IServiceDiscovery, SiNanServiceDiscovery>();
        services.AddHostedService<SiNanRegistrationHostedService>();

        return services;
    }
}
