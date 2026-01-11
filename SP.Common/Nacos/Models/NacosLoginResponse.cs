using System.Text.Json.Serialization;

namespace SP.Common.Nacos.Models;

public sealed class NacosLoginResponse
{
    [JsonPropertyName("accessToken")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("tokenTtl")]
    public int TokenTtlSeconds { get; set; }
}
