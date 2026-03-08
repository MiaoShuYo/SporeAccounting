using System.Text;
using System.Text.Json;
using SP.Common.Logger;

namespace SP.Gateway.Middleware;

/// <summary>
/// 网关入口请求/响应日志中间件（仅记录文本/JSON/XML 等可读内容，限制长度并过滤敏感头）
/// </summary>
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerService _logger;

    private const int MaxBodyChars = 4096; // 限制日志体长度，避免过大

    private static readonly HashSet<string> SensitiveHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Authorization", "Cookie", "Set-Cookie", "X-Api-Key", "X-Auth-Token"
    };

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILoggerService logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        // 记录请求
        var requestInfo = await ReadRequestAsync(context.Request);

        // 捕获响应
        var originalBodyStream = context.Response.Body;
        await using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
        }
        finally
        {
            var responseInfo = await ReadResponseAsync(context.Response);
            // 写回响应流
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await context.Response.Body.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;

            var log = new
            {
                Type = "Upstream",
                Request = requestInfo,
                Response = responseInfo,
                User = new
                {
                    UserId = context.User?.FindFirst("UserId")?.Value,
                    UserName = context.User?.FindFirst("UserName")?.Value,
                    Email = context.User?.FindFirst("Email")?.Value,
                    IsAuthenticated = context.User?.Identity?.IsAuthenticated
                },
                Timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(log, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // 成功响应记录为信息级别，非成功为警告级别
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 400)
                _logger.LogInformation("上游请求响应: {Details}", json);
            else
                _logger.LogWarning("上游请求响应(非成功): {Details}", json);
        }
    }

    private static async Task<object> ReadRequestAsync(HttpRequest request)
    {
        request.EnableBuffering();

        string? body = null;
        if (IsTextBased(request.ContentType))
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var text = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);
            body = Truncate(text, MaxBodyChars);
        }

        var headers = request.Headers
            .Where(h => !SensitiveHeaders.Contains(h.Key))
            .ToDictionary(h => h.Key, h => string.Join(",", h.Value));

        return new
        {
            Method = request.Method,
            Path = request.Path.ToString(),
            QueryString = request.QueryString.ToString(),
            Headers = headers,
            Body = body
        };
    }

    private static async Task<object> ReadResponseAsync(HttpResponse response)
    {
        string? body = null;
        if (IsTextBased(response.ContentType))
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(response.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var text = await reader.ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            body = Truncate(text, MaxBodyChars);
        }

        var headers = response.Headers
            .Where(h => !SensitiveHeaders.Contains(h.Key))
            .ToDictionary(h => h.Key, h => string.Join(",", h.Value));

        return new
        {
            StatusCode = response.StatusCode,
            Headers = headers,
            Body = body
        };
    }

    private static bool IsTextBased(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType)) return false;
        contentType = contentType.ToLowerInvariant();
        return contentType.StartsWith("text/")
               || contentType.Contains("json")
               || contentType.Contains("xml")
               || contentType.Contains("javascript")
               || contentType.Contains("html")
               || contentType.Contains("plain")
               || contentType.Contains("csv");
    }

    private static string Truncate(string input, int max)
    {
        if (string.IsNullOrEmpty(input)) return input ?? string.Empty;
        return input.Length <= max ? input : input.Substring(0, max);
    }
}