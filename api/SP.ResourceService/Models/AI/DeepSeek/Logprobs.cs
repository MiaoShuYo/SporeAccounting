using System.Text.Json.Serialization;

namespace SP.ResourceService.Models.AI.DeepSeek;

/// <summary>
/// 模该 choice 的对数概率信息
/// </summary>
public class Logprobs
{
    /// <summary>
    /// 一个包含输出 token 对数概率信息的列表
    /// </summary>
    [JsonPropertyName("content")]
    public List<LogprobItem> Content { get; set; }
    /// <summary>
    /// 一个包含输出 token 对数概率信息的列表
    /// </summary>
    [JsonPropertyName("reasoning_content")]
    public List<LogprobItem> ReasoningContent { get; set; }
}