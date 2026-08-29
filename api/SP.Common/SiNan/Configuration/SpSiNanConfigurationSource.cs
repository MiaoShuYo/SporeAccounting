using Microsoft.Extensions.Configuration;

namespace SP.Common.SiNan.Configuration;

internal sealed class SpSiNanConfigurationSource : IConfigurationSource
{
    private readonly SiNanOptions _options;

    public SpSiNanConfigurationSource(SiNanOptions options) => _options = options;

    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new SpSiNanConfigurationProvider(_options);
}
