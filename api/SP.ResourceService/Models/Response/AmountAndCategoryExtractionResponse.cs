using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace SP.ResourceService.Models.Response;

/// <summary>
/// 金额和消费类型提取结果
/// </summary>
public class AmountAndCategoryExtractionResponse
{
    /// <summary>
    /// 消费类型
    /// </summary>
    [JsonPropertyName("category")]
    public string? Category { get; set; }
    
    /// <summary>
    /// 消费金额
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal? Amount { get; set; }
    
    /// <summary>
    /// 币种
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}