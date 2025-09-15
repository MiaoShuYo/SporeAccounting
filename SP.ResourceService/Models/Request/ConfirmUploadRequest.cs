namespace SP.ResourceService.Models.Request;

/// <summary>
/// 文件上传确认请求
/// </summary>
public class ConfirmUploadRequest
{
    /// <summary>
    /// 对象名称
    /// </summary>
    public string ObjectName { get; set; } = string.Empty;

    /// <summary>
    /// 是否公开
    /// </summary>
    public bool IsPublic { get; set; } = true;

    /// <summary>
    /// 原始文件名
    /// </summary>
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>
    /// 内容类型
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小
    /// </summary>
    public long FileSize { get; set; }
}