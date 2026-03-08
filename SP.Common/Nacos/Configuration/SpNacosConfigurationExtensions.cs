using Microsoft.Extensions.Configuration;

namespace SP.Common.Nacos.Configuration;

public static class SpNacosConfigurationExtensions
{
    /// <summary>
    /// 从 Nacos 配置中心加载 Listeners 对应的配置，并支持轮询刷新。
    /// 生产环境建议通过 NACOS_USERNAME / NACOS_PASSWORD 环境变量覆盖凭证，
    /// 避免将明文密码提交到源代码仓库。
    /// </summary>
    public static IConfigurationBuilder AddSpNacosConfiguration(this IConfigurationBuilder builder, IConfiguration nacosSection)
    {
        var options = new NacosOptions();
        nacosSection.Bind(options);

        // 支持通过环境变量覆盖 Nacos 鉴权凭证（优先级高于 appsettings.json）
        // 生产部署时设置 NACOS_USERNAME 和 NACOS_PASSWORD 环境变量即可
        var envUsername = Environment.GetEnvironmentVariable("NACOS_USERNAME");
        var envPassword = Environment.GetEnvironmentVariable("NACOS_PASSWORD");
        if (!string.IsNullOrWhiteSpace(envUsername))
            options.Username = envUsername;
        if (!string.IsNullOrWhiteSpace(envPassword))
            options.Password = envPassword;

        builder.Add(new SpNacosConfigurationSource(options));
        return builder;
    }
}
