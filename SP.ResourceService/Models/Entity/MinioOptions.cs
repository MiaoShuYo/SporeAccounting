namespace SP.ResourceService.Models.Entity;

/// <summary>
/// Minio 配置选项
/// </summary>
public class MinioOptions
{
    /// <summary>
    /// Minio 服务端点
    /// </summary>
    public string Endpoint { get; set; } = "";

    /// <summary>
    /// 访问密钥 AccessKey
    /// </summary>
    public string AccessKey { get; set; } = "";

    /// <summary>
    /// 密钥 SecretKey
    /// </summary>
    public string SecretKey { get; set; } = "";

    /// <summary>
    /// 是否使用 SSL 连接
    /// </summary>
    public bool WithSSL { get; set; } = false;

    /// <summary>
    /// 公有桶名称
    /// </summary>
    public string PublicBucket { get; set; } = "public";

    /// <summary>
    /// 私有桶名称
    /// </summary>
    public string PrivateBucket { get; set; } = "private";

    /// <summary>
    /// 公有桶基础访问地址（可选）
    /// </summary>
    public string? PublicBaseUrl { get; set; }

    /// <summary>
    /// 私有桶预签名URL默认过期秒数
    /// </summary>
    public int PresignedExpireSeconds { get; set; } = 3600;
    
    /// <summary>
    /// 上传令牌过期秒数
    /// </summary>
    public int UploadTokenExpireSeconds {get; set;} = 300;
}