namespace SP.ResourceService.Models.Config;

/// <summary>
/// OCR 配置选项
/// </summary>
public class OcrOptions
{
    /// <summary>
    /// OCR供应商
    /// </summary>
    public string Provider { get; set; } = "OpenAiCompatible";

    /// <summary>
    /// OCR模型
    /// </summary>
    public string Model { get; set; } = "";

    /// <summary>
    /// OCR提示词
    /// </summary>
    public string Prompt { get; set; } = "请识别图片中的全部文字，只返回JSON对象，格式为：{\"text\":\"完整文字\",\"lines\":[\"逐行文字\"]}";

    /// <summary>
    /// 超时时间（秒）
    /// </summary>
    public int TimeoutSeconds { get; set; } = 60;

    /// <summary>
    /// 最大Token数
    /// </summary>
    public int MaxTokens { get; set; } = 4096;

    /// <summary>
    /// 采样温度
    /// </summary>
    public double Temperature { get; set; } = 0d;

    /// <summary>
    /// 图片细节级别
    /// </summary>
    public string ImageDetail { get; set; } = "auto";
}