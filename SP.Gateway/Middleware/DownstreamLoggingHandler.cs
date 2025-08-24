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

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
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
                    Upstream = upstream == null ? null : new
                    {
                        Method = upstream.Method,
                        Path = upstream.Path.ToString(),
                        QueryString = upstream.QueryString.ToString(),
                        Headers = upstream.Headers
                            .Where(h => !SensitiveHeaders.Contains(h.Key))
                            .ToDictionary(h => h.Key, h => string.Join(",", h.Value))
                    },
                    Downstream = new
                    {
                        Method = request.Method.Method,
                        Url = request.RequestUri?.ToString(),
                        StatusCode = (int)response.StatusCode,
                        Reason = response.ReasonPhrase,
                        ResponseHeaders = response.Headers
                            .Where(h => !SensitiveHeaders.Contains(h.Key))
                            .ToDictionary(h => h.Key, h => string.Join(",", h.Value)),
                        ContentHeaders = response.Content?.Headers?
                            .Where(h => !SensitiveHeaders.Contains(h.Key))
                            .ToDictionary(h => h.Key, h => string.Join(",", h.Value))
                    },
                    User = httpContext == null ? null : new
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
            }

            return response;
        }
        catch (Exception ex)
        {
            _loggerService.LogError(ex, "调用下游服务发生异常: {Method} {Url}", request.Method.Method, request.RequestUri?.ToString());
            throw;
        }
    }
}