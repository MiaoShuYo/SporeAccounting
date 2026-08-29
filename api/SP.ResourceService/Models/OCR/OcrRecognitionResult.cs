namespace SP.ResourceService.Models.OCR;

/// <summary>
/// OCR识别结果
/// </summary>
public class OcrRecognitionResult
{
    /// <summary>
    /// 识别到的完整文字
    /// </summary>
    public string RecognizedText { get; set; } = "";

    /// <summary>
    /// 逐行文字
    /// </summary>
    public List<string> Lines { get; set; } = new List<string>();

    /// <summary>
    /// OCR供应商
    /// </summary>
    public string Provider { get; set; } = "";

    /// <summary>
    /// 原始响应内容
    /// </summary>
    public string RawResponse { get; set; } = "";
}