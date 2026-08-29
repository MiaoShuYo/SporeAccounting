using Microsoft.Extensions.Configuration;

namespace SP.Common.SiNan.Configuration;

public static class SpSiNanConfigurationExtensions
{
    /// <summary>
    /// 从 SiNan 配置中心加载 Listeners 对应的配置，并支持轮询刷新。
    /// 生产环境建议通过环境变量 SINAN_APIKEY 覆盖 ApiKey，避免明文提交到仓库。
    /// </summary>
    /// <param name="builder">IConfigurationBuilder</param>
    /// <param name="sinanSection">appsettings.json 中的 sinan 配置节</param>
    public static IConfigurationBuilder AddSpSiNanConfiguration(
        this IConfigurationBuilder builder,
        IConfiguration sinanSection)
    {
        var options = new SiNanOptions();
        sinanSection.Bind(options);

        // 支持通过环境变量覆盖 ApiKey（优先级高于 appsettings.json）
        var envApiKey = Environment.GetEnvironmentVariable("SINAN_APIKEY");
        if (!string.IsNullOrWhiteSpace(envApiKey))
            options.ApiKey = envApiKey;

        builder.Add(new SpSiNanConfigurationSource(options));
        return builder;
    }
}
