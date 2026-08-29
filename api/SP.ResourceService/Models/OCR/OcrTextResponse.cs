using System.Text.Json.Serialization;

namespace SP.ResourceService.Models.OCR;

/// <summary>
/// OCR模型返回的结构化文字
/// </summary>
public class OcrTextResponse
{
    /// <summary>
    /// 完整文字
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = "";

    /// <summary>
    /// 逐行文字
    /// </summary>
    [JsonPropertyName("lines")]
    public List<string> Lines { get; set; } = new List<string>();
}