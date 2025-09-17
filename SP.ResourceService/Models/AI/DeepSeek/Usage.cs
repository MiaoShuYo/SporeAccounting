using System.Text.Json.Serialization;

namespace SP.ResourceService.Models.AI.DeepSeek;

/// <summary>
/// 用量信息
/// </summary>
public class Usage
{
    /// <summary>
    /// 请求中提示词（prompt）所使用的 token 数
    /// </summary>
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    /// <summary>
    /// 生成的回答（completion）所使用的 token 数
    /// </summary>
    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }

    /// <summary>
    /// 总共使用的 token 数
    /// </summary>
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }

    /// <summary>
    /// 提示词（prompt）中各部分所使用的 token 数详情
    /// </summary>
    [JsonPropertyName("prompt_tokens_details")]
    public PromptTokensDetails PromptTokensDetails { get; set; }

    /// <summary>
    /// 提示词缓存命中所使用的 token 数
    /// </summary>
    [JsonPropertyName("prompt_cache_hit_tokens")]
    public int PromptCacheHitTokens { get; set; }

    /// <summary>
    /// 提示词缓存未命中所使用的 token 数
    /// </summary>
    [JsonPropertyName("prompt_cache_miss_tokens")]
    public int PromptCacheMissTokens { get; set; }
}