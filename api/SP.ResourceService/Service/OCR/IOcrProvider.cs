using SP.ResourceService.Models.OCR;

namespace SP.ResourceService.Service.OCR;

/// <summary>
/// OCR供应商接口
/// </summary>
public interface IOcrProvider
{
    /// <summary>
    /// 识别图片文字
    /// </summary>
    /// <param name="image">图片字节</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>OCR识别结果</returns>
    Task<OcrRecognitionResult> RecognizeAsync(byte[] image, CancellationToken cancellationToken = default);
}