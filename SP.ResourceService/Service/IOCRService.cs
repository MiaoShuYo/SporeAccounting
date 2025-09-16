namespace SP.ResourceService.Service;

/// <summary>
/// OCR服务
/// </summary>
public interface IOCRService
{
    /// <summary>
    /// 识别图片中的文字
    /// </summary>
    /// <param name="fileId">图片文件id</param>
    /// <returns></returns>
    Task RecognizeTextAsync(long fileId);
    
    /// <summary>
    /// 获取识别到的图片文字
    /// </summary>
    /// <param name="fileId">图片文件id</param>
    /// <returns>识别结果文本</returns>
    Task<string?> GetRecognizedTextAsync(long fileId);
}