using System.Text.Json.Serialization;

namespace SP.ResourceService.Models.AI.DeepSeek;

/// <summary>
/// DeepSeek消息
/// </summary>
public class Message
{
    /// <summary>
    /// 消息内容
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }
    /// <summary>
    /// 角色
    /// </summary>
    [JsonPropertyName("role")]
    public string Role { get; set; }
}