using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Model;
using SP.ResourceService.DB;
using SP.ResourceService.Models.Entity;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SP.ResourceService.Models.Config;
using SP.ResourceService.Models.Request;
using SP.ResourceService.Models.Response;

namespace SP.ResourceService.Service.Impl;

/// <summary>
/// MinIO oss 实现类
/// </summary>
public class MinioOssServiceImpl : IOssService
{
    /// <summary>
    /// MinIO 客户端
    /// </summary>
    private readonly IMinioClient _client;

    /// <summary>
    /// MinIO 配置选项
    /// </summary>
    private readonly IOptions<MinioOptions> _options;

    /// <summary>
    /// 日志记录器
    /// </summary>
    private readonly ILogger<MinioOssServiceImpl> _logger;

    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly ResourceServiceDbContext _dbContext;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    /// <param name="dbContext"></param>
    public MinioOssServiceImpl(IOptions<MinioOptions> options, ILogger<MinioOssServiceImpl> logger,
        ResourceServiceDbContext dbContext)
    {
        _options = options;
        _logger = logger;
        _dbContext = dbContext;

        try
        {
            // 验证配置
            ValidateConfiguration(options.Value);
            // 处理端点URL格式 - MinIO客户端期望的是主机名和端口，而不是完整的URL
            var endpoint = ProcessEndpoint(options.Value.Endpoint);
            // 初始化 MinIO 客户端
            var clientBuilder = new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(options.Value.AccessKey, options.Value.SecretKey);

            if (options.Value.WithSSL)
            {
                clientBuilder = clientBuilder.WithSSL();
            }

            // 构建客户端
            _client = clientBuilder.Build();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "构建客户端失败");
            throw;
        }
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="file">文件</param>
    /// <param name="isPublic">是否公开</param>
    /// <param name="ct">取消令牌</param>
    /// <returns></returns>
    public async Task UploadAsync(IFormFile file, bool isPublic = true, CancellationToken ct = default)
    {
        using var streamRead = file.OpenReadStream();
        var objectName = $"{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
        var bucket = isPublic ? _options.Value.PublicBucket : _options.Value.PrivateBucket;
        await EnsureBucketAsync(bucket, ct);

        var putArgs = new PutObjectArgs();
        // 计算大小（若不可Seek，先缓冲）
        long size;
        if (streamRead.CanSeek)
        {
            size = streamRead.Length - streamRead.Position;
            putArgs.WithStreamData(streamRead);
        }
        else
        {
            var ms = new MemoryStream();
            await streamRead.CopyToAsync(ms, ct);
            ms.Position = 0;
            var stream = streamRead;
            stream = ms;
            size = ms.Length;
            putArgs.WithStreamData(stream);
        }

        putArgs.WithBucket(bucket)
            .WithObject(objectName)
            .WithObjectSize(size)
            .WithContentType(file.ContentType);

        await _client.PutObjectAsync(putArgs, ct);
        Files fileInfo = new Files
        {
            ObjectName = objectName,
            IsPublic = isPublic,
            Size = size,
            ContentType = file.ContentType,
            OriginalName = file.FileName
        };
        SettingCommProperty.Create(fileInfo);
        _dbContext.Files.Add(fileInfo);
        await _dbContext.SaveChangesAsync(ct);
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
    /// <param name="fileId">文件id</param>
    /// <returns></returns>
    public async Task<string> GetUrlAsync(long fileId)
    {
        // 查询文件信息
        Files? file = _dbContext.Files.FirstOrDefault(f=>f.Id==fileId && f.IsDeleted== false);
        if (file == null)
        {
            throw new NotFoundException("文件不存在");
        }

        string bucket = "";
        string objectName = file.ObjectName;
        if (file.IsPublic)
        {
            bucket = _options.Value.PublicBucket;
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
            bucket = _options.Value.PrivateBucket;
            var expirySeconds = _options.Value.PresignedExpireSeconds;
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
    /// <param name="fileId">文件id</param>
    /// <param name="ct">取消令牌</param>
    /// <returns></returns>
    public async Task DeleteAsync(long fileId, CancellationToken ct = default)
    {
        // 查询文件
        Files? fileInfo = _dbContext.Files.FirstOrDefault(f => f.IsDeleted == false && f.Id == fileId);
        if (fileInfo == null)
        {
            throw new NotFoundException("文件不存在");
        }

        var bucket = fileInfo.IsPublic ? _options.Value.PublicBucket : _options.Value.PrivateBucket;
        await EnsureBucketAsync(bucket, ct);

        var rmArgs = new RemoveObjectArgs()
            .WithBucket(bucket)
            .WithObject(fileInfo.ObjectName);

        await _client.RemoveObjectAsync(rmArgs, ct);
        // 删除数据库记录
        SettingCommProperty.Delete(fileInfo);
        await _dbContext.SaveChangesAsync(ct);
    }

    /// <summary>
    /// 生成前端直传的预签名 PUT URL
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="isPublic"></param>
    /// <param name="ct"></param>
    public async Task<PresignedURLResponse> GetPresignedPutUrlAsync(string fileName, bool isPublic,
        CancellationToken ct = default)
    {
        // 拼接日期路径和唯一标识
        string objectName = $"{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid():N}{Path.GetExtension(fileName)}";
        var bucket = isPublic ? _options.Value.PublicBucket : _options.Value.PrivateBucket;
        await EnsureBucketAsync(bucket, ct);

        int expirySeconds = _options.Value.UploadTokenExpireSeconds;
        var preArgs = new PresignedPutObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectName)
            .WithExpiry(expirySeconds);

        string uploadUrl = await _client.PresignedPutObjectAsync(preArgs);
        PresignedURLResponse presignedUrlResponse = new PresignedURLResponse()
        {
            UploadUrl = uploadUrl,
            ObjectName = objectName,
            OriginalFileName = fileName
        };
        return presignedUrlResponse;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task ConfirmUploadAsync(ConfirmUploadRequest request, CancellationToken ct = default)
    {
        // 验证文件是否真的存在于 MinIO 中
        var bucket = request.IsPublic ? _options.Value.PublicBucket : _options.Value.PrivateBucket;
        try
        {
            var statArgs = new StatObjectArgs()
                .WithBucket(bucket)
                .WithObject(request.ObjectName);

            var objectStat = await _client.StatObjectAsync(statArgs, ct);

            // 创建文件记录
            var fileInfo = new Files
            {
                ObjectName = request.ObjectName,
                IsPublic = request.IsPublic,
                Size = request.FileSize,
                ContentType = request.ContentType,
                OriginalName = request.OriginalFileName
            };

            SettingCommProperty.Create(fileInfo);
            _dbContext.Files.Add(fileInfo);
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            var sanitizedObjectName = request.ObjectName?.Replace("\r", "").Replace("\n", "");
            _logger.LogError(ex, "无法确认文件上传:{ObjectName}", sanitizedObjectName);
            throw new BadRequestException($"无法确认文件上传: {request.ObjectName}");
        }
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

            // 如果是公共桶，设置为公共访问策略
            if (bucket == _options.Value.PublicBucket)
            {
                await SetPublicBucketPolicyAsync(bucket, ct);
                _logger.LogInformation("Set public access policy for bucket: {Bucket}", bucket);
            }
        }
    }

    /// <summary>
    /// 为桶设置公共访问策略
    /// </summary>
    /// <param name="bucket">桶名称</param>
    /// <param name="ct">取消令牌</param>
    /// <returns></returns>
    private async Task SetPublicBucketPolicyAsync(string bucket, CancellationToken ct)
    {
        try
        {
            // 创建公共访问策略JSON
            var policy = new
            {
                Version = "2012-10-17",
                Statement = new[]
                {
                    new
                    {
                        Effect = "Allow",
                        Principal = new { AWS = new[] { "*" } },
                        Action = new[] { "s3:GetObject" },
                        Resource = new[] { $"arn:aws:s3:::{bucket}/*" }
                    }
                }
            };

            var policyJson = JsonSerializer.Serialize(policy, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            // 使用SetPolicyAsync方法设置桶策略
            var setPolicyArgs = new SetPolicyArgs()
                .WithBucket(bucket)
                .WithPolicy(policyJson);

            await _client.SetPolicyAsync(setPolicyArgs, ct);
        }
        catch (Exception ex)
        {
            // 不抛出异常，因为桶已经创建成功，只是策略设置失败
            _logger.LogError(ex, "为桶设置公共访问策略失败: {Bucket}。桶仍可用，但对象无法通过URL直接访问。", bucket);
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
            throw new ArgumentException("MinIO 端点不能为空");
        }

        if (string.IsNullOrWhiteSpace(options.AccessKey))
        {
            throw new ArgumentException("MinIO AccessKey 不能为空");
        }

        if (string.IsNullOrWhiteSpace(options.SecretKey))
        {
            throw new ArgumentException("MinIO SecretKey 不能为空");
        }

        if (string.IsNullOrWhiteSpace(options.PublicBucket))
        {
            throw new ArgumentException("MinIO 公共桶不能为空");
        }

        if (string.IsNullOrWhiteSpace(options.PrivateBucket))
        {
            throw new ArgumentException("MinIO 私有桶不能为空");
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
}