using Microsoft.AspNetCore.Mvc;
using SP.ResourceService.Models.Request;
using SP.ResourceService.Models.Response;
using SP.ResourceService.Service;

namespace SP.ResourceService.Controllers;

/// <summary>
/// 文件控制器
/// </summary>
[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    /// <summary>
    /// OSS服务
    /// </summary>
    private readonly IOssService _oss;

    /// <summary>
    /// 文件控制器构造函数
    /// </summary>
    /// <param name="oss"></param>
    public FilesController(IOssService oss)
    {
        _oss = oss;
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="file"></param>
    /// <param name="isPublic"></param>
    /// <returns></returns>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Upload(IFormFile file, [FromQuery] bool isPublic = true)
    {
        await _oss.UploadAsync(file,isPublic);
        return Ok();
    }

    /// <summary>
    /// 获取文件URL
    /// </summary>
    /// <param name="fileId">文件id</param>
    /// <returns></returns>
    [HttpGet("url")]
    public async Task<string> GetUrl([FromQuery] long fileId)
    {
        return await _oss.GetUrlAsync(fileId);
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="fileId">文件id</param>
    /// <returns></returns>
    [HttpDelete]
    public async Task Delete([FromQuery] long fileId)
    {
        await _oss.DeleteAsync(fileId);
    }

    /// <summary>
    /// 获取用于前端直传的上传凭证（预签名 PUT URL）
    /// </summary>
    /// <param name="fileName">文件名（包含扩展名，如 avatar.jpg）</param>
    /// <param name="isPublic">是否上传到公开桶（默认公开）</param>
    /// <returns>包含预签名上传 URL 和对象名的响应</returns>
    [HttpGet("upload-token")]
    public async Task<PresignedURLResponse> GetUploadToken([FromQuery] string fileName, [FromQuery] bool isPublic = true)
    {
        PresignedURLResponse presignedPutUrl = await _oss.GetPresignedPutUrlAsync(fileName, isPublic);
        return presignedPutUrl;
    }
    
    /// <summary>
    /// 确认文件上传成功
    /// </summary>
    /// <param name="request">文件确认请求</param>
    /// <returns></returns>
    [HttpPost("confirm-upload")]
    public async Task<ActionResult> ConfirmUpload([FromBody] ConfirmUploadRequest request)
    {
        await _oss.ConfirmUploadAsync(request);
        return Ok();
    }
}