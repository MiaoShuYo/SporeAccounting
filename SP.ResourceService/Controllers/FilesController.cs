using Microsoft.AspNetCore.Mvc;
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
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<string> Upload(IFormFile file, [FromQuery] bool isPublic = true,
        CancellationToken ct = default)
    {
        using var s = file.OpenReadStream();
        var objectName = $"{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
        return await _oss.UploadAsync(s, objectName, isPublic, file.ContentType, ct);
    }

    /// <summary>
    /// 获取文件URL
    /// </summary>
    /// <param name="objectName"></param>
    /// <param name="isPublic"></param>
    /// <param name="expireSeconds"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("url")]
    public async Task<string> GetUrl([FromQuery] string objectName, [FromQuery] bool isPublic = true,
        [FromQuery] int? expireSeconds = null, CancellationToken ct = default)
    {
        TimeSpan? exp = expireSeconds is > 0 ? TimeSpan.FromSeconds(expireSeconds.Value) : null;
        return await _oss.GetUrlAsync(objectName, isPublic, exp, ct);
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="objectName"></param>
    /// <param name="isPublic"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("download")]
    public async Task<IActionResult> Download([FromQuery] string objectName, [FromQuery] bool isPublic = false,
        CancellationToken ct = default)
    {
        if (isPublic)
        {
            var url = await _oss.GetUrlAsync(objectName, true, null, ct);
            return Redirect(url);
        }

        var stream = await _oss.DownloadAsync(objectName, false, ct);
        return File(stream, "application/octet-stream", Path.GetFileName(objectName));
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="objectName"></param>
    /// <param name="isPublic"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task Delete([FromQuery] string objectName, [FromQuery] bool isPublic = false,
        CancellationToken ct = default)
    {
        await _oss.DeleteAsync(objectName, isPublic, ct);
    }

    /// <summary>
    /// 获取用于前端直传的上传凭证（预签名 PUT URL）
    /// </summary>
    /// <param name="objectName">对象名称（前端准备好完整路径，如 yyyy/MM/dd/xxx.ext）</param>
    /// <param name="isPublic">是否上传到公开桶（默认公开）</param>
    /// <param name="expireSeconds">URL 过期秒数（默认配置值）</param>
    /// <param name="ct"></param>
    /// <returns>预签名上传 URL</returns>
    [HttpGet("upload-token")]
    public async Task<string> GetUploadToken([FromQuery] string objectName, [FromQuery] bool isPublic = true,
        [FromQuery] int? expireSeconds = null, CancellationToken ct = default)
    {
        TimeSpan? exp = expireSeconds is > 0 ? TimeSpan.FromSeconds(expireSeconds.Value) : null;
        return await _oss.GetPresignedPutUrlAsync(objectName, isPublic, exp, ct);
    }
}