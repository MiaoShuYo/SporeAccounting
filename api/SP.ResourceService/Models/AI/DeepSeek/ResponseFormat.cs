using System.Text.Json.Serialization;
using SP.ResourceService.Models.Enumeration;

namespace SP.ResourceService.Models.AI.DeepSeek;

/// <summary>
/// 格式化响应
/// </summary>
public class ResponseFormat
{
    /// <summary>
    /// 格式化类型
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = AIResponseFormat.Text;
}