using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using SP.ResourceService.Models.Entity;

namespace SP.ResourceService.Service.Impl;

/// <summary>
/// MinIO oss 实现类
/// </summary>
public class MinioOssService : IOssService
{
    private readonly IMinioClient _client;
    private readonly IOptions<MinioOptions> _options;
    private readonly ILogger<MinioOssService> _logger;

    public MinioOssService(IOptions<MinioOptions> options, ILogger<MinioOssService> logger)
    {
        _options = options;
        _logger = logger;

        try
        {
            // 验证配置
            ValidateConfiguration(options.Value);

            // 处理端点URL格式 - MinIO客户端期望的是主机名和端口，而不是完整的URL
            var endpoint = ProcessEndpoint(options.Value.Endpoint);

            _logger.LogInformation("Initializing MinIO client with endpoint: {Endpoint}", endpoint);

            var clientBuilder = new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(options.Value.AccessKey, options.Value.SecretKey);

            if (options.Value.WithSSL)
            {
                clientBuilder = clientBuilder.WithSSL();
            }

            _client = clientBuilder.Build();

            _logger.LogInformation("MinIO client initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize MinIO client");
            throw;
        }
    }

    /// <summary>
    /// 验证MinIO配置
    /// </summary>
    /// <param name="options">MinIO配置选项</param>
    /// <exception cref="ArgumentException">配置无效时抛出异常</exception>
    private void ValidateConfiguration(MinioOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Endpoint))
        {
            throw new ArgumentException("MinIO endpoint cannot be null or empty");
        }

        if (string.IsNullOrWhiteSpace(options.AccessKey))
        {
            throw new ArgumentException("MinIO AccessKey cannot be null or empty");
        }

        if (string.IsNullOrWhiteSpace(options.SecretKey))
        {
            throw new ArgumentException("MinIO SecretKey cannot be null or empty");
        }

        if (string.IsNullOrWhiteSpace(options.PublicBucket))
        {
            throw new ArgumentException("MinIO PublicBucket cannot be null or empty");
        }

        if (string.IsNullOrWhiteSpace(options.PrivateBucket))
        {
            throw new ArgumentException("MinIO PrivateBucket cannot be null or empty");
        }
    }

    /// <summary>
    /// 处理端点URL格式
    /// </summary>
    /// <param name="endpoint">原始端点</param>
    /// <returns>处理后的端点</returns>
    private string ProcessEndpoint(string endpoint)
    {
        // 如果端点包含协议前缀，需要移除
        if (endpoint.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
        {
            endpoint = endpoint.Substring("http://".Length);
        }
        else if (endpoint.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            endpoint = endpoint.Substring("https://".Length);
        }

        // 移除末尾的斜杠
        endpoint = endpoint.TrimEnd('/');

        return endpoint;
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="stream">文件流</param>
    /// <param name="objectName">对象名称</param>
    /// <param name="isPublic">是否公开</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="ct">取消令牌</param>
    /// <returns></returns>
    public async Task<string> UploadAsync(Stream stream, string objectName, bool isPublic, string? contentType = null,
        CancellationToken ct = default)
    {
        var bucket = isPublic ? _options.Value.PublicBucket : _options.Value.PrivateBucket;
        await EnsureBucketAsync(bucket, ct);

        // 计算大小（若不可Seek，先缓冲）
        long size;
        if (stream.CanSeek)
        {
            size = stream.Length - stream.Position;
        }
        else
        {
            var ms = new MemoryStream();
            await stream.CopyToAsync(ms, ct);
            ms.Position = 0;
            stream = ms;
            size = ms.Length;
        }

        var putArgs = new PutObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(size)
            .WithContentType(contentType ?? "application/octet-stream");

        await _client.PutObjectAsync(putArgs, ct);

        // 返回可访问链接：公开桶返回直链，私有桶返回预签名
        return await GetUrlAsync(objectName, isPublic, null, ct);
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="objectName">对象名称</param>
    /// <param name="isPublic">是否公开</param>
    /// <param name="ct">取消令牌</param>
    /// <returns></returns>
    public async Task<Stream> DownloadAsync(string objectName, bool isPublic, CancellationToken ct = default)
    {
        var bucket = isPublic ? _options.Value.PublicBucket : _options.Value.PrivateBucket;
        await EnsureBucketAsync(bucket, ct);

        var ms = new MemoryStream();
        var getArgs = new GetObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectName)
            .WithCallbackStream(s => s.CopyTo(ms));

        await _client.GetObjectAsync(getArgs, ct);
        ms.Position = 0;
        return ms;
    }

    /// <summary>
    /// 获取文件URL
    /// </summary>
    /// <param name="objectName">对象名称</param>
    /// <param name="isPublic">是否公开</param>
    /// <param name="expires">过期时间</param>
    /// <param name="ct">取消令牌</param>
    /// <returns></returns>
    public async Task<string> GetUrlAsync(string objectName, bool isPublic, TimeSpan? expires = null,
        CancellationToken ct = default)
    {
        var bucket = isPublic ? _options.Value.PublicBucket : _options.Value.PrivateBucket;

        if (isPublic)
        {
            // 公开桶：返回直链
            var baseUrl = _options.Value.PublicBaseUrl?.TrimEnd('/');
            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                return $"{baseUrl}/{bucket}/{Uri.EscapeDataString(objectName)}";
            }

            // 若未配置 PublicBaseUrl，则退回到 MinIO 原始地址
            var scheme = _options.Value.WithSSL ? "https" : "http";
            return $"{scheme}://{_options.Value.Endpoint.TrimEnd('/')}/{bucket}/{Uri.EscapeDataString(objectName)}";
        }
        else
        {
            // 私有桶：返回预签名
            var expirySeconds = (int)(expires?.TotalSeconds ?? _options.Value.PresignedExpireSeconds);
            var preArgs = new PresignedGetObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectName)
                .WithExpiry(expirySeconds);
            return await _client.PresignedGetObjectAsync(preArgs);
        }
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="objectName">对象名称</param>
    /// <param name="isPublic">是否公开</param>
    /// <param name="ct">取消令牌</param>
    /// <returns></returns>
    public async Task DeleteAsync(string objectName, bool isPublic, CancellationToken ct = default)
    {
        var bucket = isPublic ? _options.Value.PublicBucket : _options.Value.PrivateBucket;
        await EnsureBucketAsync(bucket, ct);

        var rmArgs = new RemoveObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectName);

        await _client.RemoveObjectAsync(rmArgs, ct);
    }

    /// <summary>
    /// 生成前端直传的预签名 PUT URL
    /// </summary>
    /// <param name="objectName"></param>
    /// <param name="isPublic"></param>
    /// <param name="expires"></param>
    /// <param name="ct"></param>
    public async Task<string> GetPresignedPutUrlAsync(string objectName, bool isPublic, TimeSpan? expires = null,
        CancellationToken ct = default)
    {
        var bucket = isPublic ? _options.Value.PublicBucket : _options.Value.PrivateBucket;
        await EnsureBucketAsync(bucket, ct);

        var expirySeconds = (int)(expires?.TotalSeconds ?? _options.Value.PresignedExpireSeconds);
        var preArgs = new PresignedPutObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectName)
            .WithExpiry(expirySeconds);

        return await _client.PresignedPutObjectAsync(preArgs);
    }

    /// <summary>
    /// 确保桶存在
    /// </summary>
    /// <param name="bucket">桶名称</param>
    /// <param name="ct">取消令牌</param>
    /// <returns></returns>
    private async Task EnsureBucketAsync(string bucket, CancellationToken ct)
    {
        var exists = await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucket), ct);
        if (!exists)
        {
            await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket), ct);
            _logger.LogInformation("Created bucket: {Bucket}", bucket);
        }
    }
}