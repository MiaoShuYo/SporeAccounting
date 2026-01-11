using Microsoft.Extensions.Configuration;

namespace SP.Common.Nacos.Configuration;

internal sealed class SpNacosConfigurationSource : IConfigurationSource
{
    public SpNacosConfigurationSource(NacosOptions options) => Options = options;
    public NacosOptions Options { get; }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new SpNacosConfigurationProvider(Options);
}
