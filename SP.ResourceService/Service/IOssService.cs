using SP.ResourceService.Models.Request;
using SP.ResourceService.Models.Response;

namespace SP.ResourceService.Service;

/// <summary>
/// OSS服务接口
/// </summary>
public interface IOssService
{
    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="file">文件</param>
    /// <param name="isPublic">是否公开</param>
    /// <param name="ct">取消令牌</param>
    Task UploadAsync(IFormFile file, bool isPublic = true, CancellationToken ct = default);

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="objectName">对象名称</param>
    /// <param name="isPublic">是否公开</param>
    /// <param name="ct">取消令牌</param>
    Task<Stream> DownloadAsync(string objectName, bool isPublic, CancellationToken ct = default);

    /// <summary>
    /// 获取文件URL
    /// </summary>
    /// <param name="fileId">文件id</param>
    Task<string> GetUrlAsync(long fileId);

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="fileId">文件id</param>
    /// <param name="ct">取消令牌</param>
    Task DeleteAsync(long fileId, CancellationToken ct = default);

    /// <summary>
    /// 生成用于前端直传的预签名 PUT URL
    /// </summary>
    /// <param name="fileName">对象名称</param>
    /// <param name="isPublic">是否公开桶</param>
    /// <param name="ct">取消令牌</param>
    Task<PresignedURLResponse> GetPresignedPutUrlAsync(string fileName, bool isPublic, CancellationToken ct = default);

    /// <summary>
    /// 文件上传确认
    /// </summary>
    /// <param name="request">上传确认请求</param>
    /// <returns></returns>
    Task ConfirmUploadAsync(ConfirmUploadRequest request, CancellationToken ct = default);
}