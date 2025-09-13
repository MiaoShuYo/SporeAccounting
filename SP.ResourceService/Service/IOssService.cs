namespace SP.ResourceService.Service;

/// <summary>
/// OSS服务接口
/// </summary>
public interface IOssService
{
    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="stream">文件流</param>
    /// <param name="objectName">对象名称</param>
    /// <param name="isPublic">是否公开</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="ct">取消令牌</param>
    Task<string> UploadAsync(Stream stream, string objectName, bool isPublic, string? contentType = null,
        CancellationToken ct = default);

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
    /// <param name="objectName">对象名称</param>
    /// <param name="isPublic">是否公开</param>
    /// <param name="expires">过期时间</param>
    Task<string> GetUrlAsync(string objectName, bool isPublic, TimeSpan? expires = null,
        CancellationToken ct = default);

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="objectName">对象名称</param>
    /// <param name="isPublic">是否公开</param>
    /// <param name="ct">取消令牌</param>
    Task DeleteAsync(string objectName, bool isPublic, CancellationToken ct = default);

    /// <summary>
    /// 生成用于前端直传的预签名 PUT URL
    /// </summary>
    /// <param name="objectName">对象名称</param>
    /// <param name="isPublic">是否公开桶</param>
    /// <param name="expires">过期时间</param>
    /// <param name="ct">取消令牌</param>
    Task<string> GetPresignedPutUrlAsync(string objectName, bool isPublic, TimeSpan? expires = null,
        CancellationToken ct = default);
}