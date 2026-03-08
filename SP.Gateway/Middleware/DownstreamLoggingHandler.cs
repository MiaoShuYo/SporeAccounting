using System.Net;
using System.Text;
using System.Text.Json;
using SP.Common.Logger;

namespace SP.Gateway.Middleware;

/// <summary>
/// 记录下游服务错误响应与异常的 Ocelot 委托处理器
/// </summary>
public class DownstreamLoggingHandler : DelegatingHandler
{
    private static readonly HashSet<string> SensitiveHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Authorization", "Cookie", "Set-Cookie", "X-Api-Key", "X-Auth-Token"
    };

    private readonly ILogger<DownstreamLoggingHandler> _logger;
    private readonly ILoggerService _loggerService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DownstreamLoggingHandler(
        ILogger<DownstreamLoggingHandler> logger,
        ILoggerService loggerService,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _loggerService = loggerService;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var upstream = httpContext?.Request;

                var log = new
                {
                    Upstream = upstream == null
                        ? null
                        : new
                        {
                            Method = upstream.Method,
                            Path = upstream.Path.ToString(),
                            QueryString = upstream.QueryString.ToString(),
                            Headers = upstream.Headers
                                .Where(h => !SensitiveHeaders.Contains(h.Key))
                                .ToDictionary(h => h.Key, h => string.Join(",", h.Value.ToArray()))
                        },
                    Downstream = new
                    {
                        Method = request.Method.Method,
                        Url = request.RequestUri?.ToString(),
                        StatusCode = (int)response.StatusCode,
                        Reason = response.ReasonPhrase,
                        ResponseHeaders = response.Headers
                            .Where(h => !SensitiveHeaders.Contains(h.Key))
                            .ToDictionary(h => h.Key, h => string.Join(",", h.Value.ToArray())),
                        ContentHeaders = response.Content?.Headers?
                            .Where(h => !SensitiveHeaders.Contains(h.Key))
                            .ToDictionary(h => h.Key, h => string.Join(",", h.Value.ToArray()))
                    },
                    User = httpContext == null
                        ? null
                        : new
                        {
                            UserId = httpContext.User?.FindFirst("UserId")?.Value,
                            UserName = httpContext.User?.FindFirst("UserName")?.Value,
                            Email = httpContext.User?.FindFirst("Email")?.Value,
                            IsAuthenticated = httpContext.User?.Identity?.IsAuthenticated
                        },
                    Timestamp = DateTime.UtcNow
                };

                var json = JsonSerializer.Serialize(log, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                });

                _loggerService.LogError("下游服务返回错误: {Details}", json);

                // 统一包装下游错误响应：不透传下游详细错误，避免信息泄露
                var errorMessage = MapDefaultMessage(response.StatusCode);
                var unified = SerializeUnifiedError(response.StatusCode, errorMessage);
                response.Content = new StringContent(unified, Encoding.UTF8, "application/json");
            }

            return response;
        }
        catch (Exception ex)
        {
            var safeUrl = request.RequestUri?.ToString() ?? string.Empty;
            _loggerService.LogError(ex, "调用下游服务发生异常: {Method} {Url}", request.Method.Method, safeUrl);

            // 根据异常类型构造统一错误响应
            var (statusCode, message) = MapExceptionToStatusAndMessage(ex, request);

            var unified = SerializeUnifiedError(statusCode, message, ex);
            var errorResponse = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(unified, Encoding.UTF8, "application/json"),
                RequestMessage = request
            };
            return errorResponse;
        }
    }

    private static (HttpStatusCode Status, string Message) MapExceptionToStatusAndMessage(Exception exception,
        HttpRequestMessage request)
    {
        if (exception is TaskCanceledException)
        {
            return (HttpStatusCode.GatewayTimeout, exception.Message);
        }

        if (exception is HttpRequestException httpEx)
        {
            if (httpEx.StatusCode.HasValue)
            {
                return (httpEx.StatusCode.Value, httpEx.Message);
            }

            return (HttpStatusCode.BadGateway, httpEx.Message);
        }

        return (HttpStatusCode.BadGateway, exception.Message);
    }

    private static string MapDefaultMessage(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.NotFound => "下游服务未找到",
            HttpStatusCode.Unauthorized => "未授权访问下游服务",
            HttpStatusCode.Forbidden => "禁止访问下游服务",
            HttpStatusCode.RequestTimeout => "下游服务请求超时",
            HttpStatusCode.GatewayTimeout => "下游服务请求超时",
            HttpStatusCode.ServiceUnavailable => "下游服务不可用",
            HttpStatusCode.BadGateway => "下游服务不可用",
            _ => "下游服务返回错误"
        };
    }

    private static string SerializeUnifiedError(HttpStatusCode statusCode, string message, Exception? ex = null)
    {
        var payload = new
        {
            statusCode,
            errorMessage = message,
#if DEBUG
            stackTrace = ex?.ToString()
#endif
        };

        return JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}