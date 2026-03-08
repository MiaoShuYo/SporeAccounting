using Microsoft.AspNetCore.Mvc;
using SP.ResourceService.Service;

namespace SP.ResourceService.Controllers
{
    /// <summary>
    /// OCR控制器
    /// </summary>
    [Route("api/ocr")]
    [ApiController]
    public class OCRController : ControllerBase
    {
        /// <summary>
        /// ocr服务
        /// </summary>
        private readonly IOCRService _ocrService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ocrService"></param>
        public OCRController(IOCRService ocrService)
        {
            _ocrService = ocrService;
        }

        /// <summary>
        /// 识别图片中的文字
        /// </summary>
        /// <param name="fileId">图片文件id</param>
        /// <returns></returns>
        [HttpGet("recognize")]
        public async Task<ActionResult> RecognizeText([FromQuery] long fileId)
        {
            await _ocrService.RecognizeTextAsync(fileId);
            return Ok();
        }

        /// <summary>
        /// 获取识别到的图片文字
        /// </summary>
        /// <param name="fileId">图片文件id</param>
        /// <returns></returns>
        [HttpGet("text")]
        public async Task<ActionResult<string>> GetRecognizedText([FromQuery] long fileId)
        {
            string? text = await _ocrService.GetRecognizedTextAsync(fileId);
            return Ok(text);
        }
    }
}