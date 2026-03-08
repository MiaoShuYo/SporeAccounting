namespace SP.ResourceService.Models.Config;

/// <summary>
/// 百度OCR配置选项
/// </summary>
public class BaiduOCROptions
{
    /// <summary>
    /// 应用ID
    /// </summary>
    public string AppId { get; set; } = "";

    /// <summary>
    /// API Key
    /// </summary>
    public string APIKey { get; set; } = "";

    /// <summary>
    /// Secret Key
    /// </summary>
    public string SecretKey { get; set; } = "";
}