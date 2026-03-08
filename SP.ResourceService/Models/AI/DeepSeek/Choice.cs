using System.Text.Json.Serialization;

namespace SP.ResourceService.Models.AI.DeepSeek;

public class Choice
{
    /// <summary>
    /// 该 completion 在模型生成的 completion 的选择列表中的索引
    /// </summary>
    [JsonPropertyName("index")]
    public int Index { get; set; }
    /// <summary>
    /// 模型生成的 completion 消息
    /// </summary>
    [JsonPropertyName("message")]
    public Message Message { get; set; }
    /// <summary>
    /// 该 choice 的对数概率信息
    /// </summary>
    [JsonPropertyName("logprobs")]
    public Logprobs Logprobs { get; set; }
    /// <summary>
    /// 模型停止生成 token 的原因
    /// </summary>
    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; }
}