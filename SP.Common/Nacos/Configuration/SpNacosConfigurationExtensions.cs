using Microsoft.Extensions.Configuration;

namespace SP.Common.Nacos.Configuration;

public static class SpNacosConfigurationExtensions
{
    /// <summary>
    /// 从 Nacos 配置中心加载 Listeners 对应的配置，并支持轮询刷新。
    /// </summary>
    public static IConfigurationBuilder AddSpNacosConfiguration(this IConfigurationBuilder builder, IConfiguration nacosSection)
    {
        var options = new NacosOptions();
        nacosSection.Bind(options);
        builder.Add(new SpNacosConfigurationSource(options));
        return builder;
    }
}
