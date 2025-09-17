using System.Text.Json.Serialization;

namespace SP.ResourceService.Models.AI.DeepSeek;

/// <summary>
/// 提示词令牌详情
/// </summary>
public class PromptTokensDetails
{
    /// <summary>
    /// 提示词中用户消息所使用的令牌数
    /// </summary>
    [JsonPropertyName("cached_tokens")]
    public int CachedTokens { get; set; }
}