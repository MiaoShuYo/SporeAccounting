using System.Text.Json.Serialization;

namespace SP.ResourceService.Models.AI.DeepSeek;

/// <summary>
/// DeepSeek 聊天响应
/// </summary>
public class DeepSeekChatResponse
{
    /// <summary>
    /// 该对话的唯一标识符
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }
    /// <summary>
    /// 对象的类型
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; }
    /// <summary>
    /// 创建聊天完成时的 Unix 时间戳（以秒为单位）
    /// </summary>
    [JsonPropertyName("created")]
    public long Created { get; set; }
    /// <summary>
    /// 生成该 completion 的模型名
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; }
    /// <summary>
    /// 模型生成的 completion 的选择列表
    /// </summary>
    [JsonPropertyName("choices")]
    public List<Choice> Choices { get; set; }
    /// <summary>
    /// 该对话补全请求的用量信息
    /// </summary>
    [JsonPropertyName("usage")]
    public Usage Usage { get; set; }
    /// <summary>
    /// 模型运行所依赖的后端配置
    /// </summary>
    [JsonPropertyName("system_fingerprint")]
    public string SystemFingerprint { get; set; }
}