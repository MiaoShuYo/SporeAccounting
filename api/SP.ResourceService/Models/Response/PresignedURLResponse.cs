namespace SP.ResourceService.Models.Response;

/// <summary>
/// 预签名URL响应
/// </summary>
public class PresignedURLResponse
{
    /// <summary>
    /// 上传URL
    /// </summary>
    public string UploadUrl;

    /// <summary>
    /// 对象名称
    /// </summary>
    public string ObjectName;

    /// <summary>
    /// 原始文件名
    /// </summary>
    public string OriginalFileName;
}