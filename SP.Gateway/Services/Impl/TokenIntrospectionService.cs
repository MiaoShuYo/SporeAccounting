using System.Text;
using System.Text.Json;
using SP.Gateway.Models;

namespace SP.Gateway.Services.Impl;

/// <summary>
/// 令牌内省服务实现
/// </summary>
public class TokenIntrospectionService : ITokenIntrospectionService
{
    // HttpClient 用于发送 HTTP 请求
    private readonly HttpClient _httpClient;

    // 日志记录器
    private readonly ILogger<TokenIntrospectionService> _logger;

    // 网关配置服务
    private readonly IGatewayConfigService _configService;

    // 配置服务
    private readonly IConfiguration _configuration;

    /// <summary>
    /// 构造函数，注入依赖
    /// </summary>
    /// <param name="httpClient">HTTP 客户端</param>
    /// <param name="logger">日志记录器</param>
    /// <param name="configService">网关配置服务</param>
    /// <param name="configuration">配置服务</param>
    public TokenIntrospectionService(
        HttpClient httpClient,
        ILogger<TokenIntrospectionService> logger,
        IGatewayConfigService configService,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configService = configService;
        _configuration = configuration;
    }

    /// <summary>
    /// 异步令牌内省方法，校验令牌有效性并解析相关信息
    /// </summary>
    /// <param name="token">待校验的令牌</param>
    /// <param name="identityServiceUrl">身份服务地址</param>
    /// <returns>令牌内省结果，若无效则返回 null</returns>
    public async Task<TokenIntrospectionResponse?> IntrospectTokenAsync(string token, string identityServiceUrl)
    {
        try
        {
            // 获取身份服务配置
            var config = await _configService.GetIdentityServiceConfigAsync();

            // 构造内省接口地址
            var introspectionUrl = $"{identityServiceUrl.TrimEnd('/')}/api/auth/introspect";
            // 构造请求体
            var requestData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("token", token)
            });

            // 发送 POST 请求，确保 Content-Type 为 application/x-www-form-urlencoded
            var request = new HttpRequestMessage(HttpMethod.Post, introspectionUrl)
            {
                Content = requestData
            };
            request.Content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            // 添加网关签名和匿名标识
            request.Headers.Add("X-Anonymous", "true");
            request.Headers.Add("X-Gateway-Signature", GenerateGatewaySignature());

            var response = await _httpClient.SendAsync(request);

            // 请求失败则记录警告并返回 null
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("令牌内省请求失败，状态码: {StatusCode}", response.StatusCode);
                return null;
            }

            // 读取响应内容
            var content = await response.Content.ReadAsStringAsync();
            var introspectionResponse = JsonSerializer.Deserialize<JsonElement>(content);

            // 检查令牌是否有效
            if (!introspectionResponse.TryGetProperty("active", out var activeProperty) ||
                !activeProperty.GetBoolean())
            {
                _logger.LogDebug("令牌内省结果显示令牌无效");
                return null;
            }

            // 构造令牌内省结果对象
            var result = new TokenIntrospectionResponse
            {
                IsActive = true,
                Subject = GetStringProperty(introspectionResponse, "sub"),
                Username = GetStringProperty(introspectionResponse, "username"),
                Email = GetStringProperty(introspectionResponse, "email"),
                Scope = GetStringProperty(introspectionResponse, "scope"),
                ClientId = GetStringProperty(introspectionResponse, "client_id"),
                ExpiresAt = GetLongProperty(introspectionResponse, "exp")
            };

            // 解析角色信息
            if (introspectionResponse.TryGetProperty("roles", out var rolesProperty))
            {
                result.Roles = rolesProperty.EnumerateArray()
                    .Select(r => r.GetString())
                    .Where(r => !string.IsNullOrEmpty(r))
                    .ToList()!;
            }

            // 解析权限信息
            if (introspectionResponse.TryGetProperty("permissions", out var permissionsProperty))
            {
                result.Permissions = permissionsProperty.EnumerateArray()
                    .Select(p => p.GetString())
                    .Where(p => !string.IsNullOrEmpty(p))
                    .ToList()!;
            }

            _logger.LogDebug("令牌内省成功，用户: {Username}, 客户端: {ClientId}",
                result.Username, result.ClientId);

            return result;
        }
        catch (Exception ex)
        {
            // 异常处理，记录错误日志
            _logger.LogError(ex, "令牌内省时发生错误");
            return null;
        }
    }

    /// <summary>
    /// 从 JsonElement 获取字符串属性
    /// </summary>
    /// <param name="element">JsonElement 对象</param>
    /// <param name="propertyName">属性名</param>
    /// <returns>属性值或 null</returns>
    private static string? GetStringProperty(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var property) ? property.GetString() : null;
    }

    /// <summary>
    /// 从 JsonElement 获取长整型属性
    /// </summary>
    /// <param name="element">JsonElement 对象</param>
    /// <param name="propertyName">属性名</param>
    /// <returns>属性值或 null</returns>
    private static long? GetLongProperty(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var property) ? property.GetInt64() : null;
    }

    /// <summary>
    /// 生成网关签名
    /// </summary>
    /// <returns></returns>
    private string GenerateGatewaySignature()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var secret = _configuration["GatewaySecret"] ?? "SP_Gateway_Secret_2024";
        var signature = $"{timestamp}.{secret}";
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(signature));
    }
}