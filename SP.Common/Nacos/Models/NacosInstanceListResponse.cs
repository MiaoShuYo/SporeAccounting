using System.Text.Json.Serialization;

namespace SP.Common.Nacos.Models;

public sealed class NacosInstanceListResponse
{
    [JsonPropertyName("hosts")]
    public List<NacosInstance>? Hosts { get; set; }
}
