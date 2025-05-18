using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SP.Common;

/// <summary>
/// 令牌客户端，用于获取访问令牌
/// </summary>
public class TokenClient
{
    private readonly HttpClient _httpClient;
    private readonly string _tokenEndpoint;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _scope;
    
    // 缓存的令牌信息
    private TokenResponse? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;
    private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="tokenEndpoint">令牌端点URL</param>
    /// <param name="clientId">客户端ID</param>
    /// <param name="clientSecret">客户端密钥</param>
    /// <param name="scope">请求范围，默认为api</param>
    public TokenClient(string tokenEndpoint, string clientId, string clientSecret, string scope = "api")
    {
        _httpClient = new HttpClient();
        _tokenEndpoint = tokenEndpoint;
        _clientId = clientId;
        _clientSecret = clientSecret;
        _scope = scope;
    }

    /// <summary>
    /// 获取访问令牌
    /// </summary>
    /// <param name="forceRefresh">是否强制刷新令牌</param>
    /// <returns>访问令牌</returns>
    public async Task<string> GetAccessTokenAsync(bool forceRefresh = false)
    {
        await _lock.WaitAsync();
        
        try
        {
            // 检查缓存的令牌是否有效
            if (!forceRefresh && _cachedToken != null && DateTime.UtcNow < _tokenExpiry.AddMinutes(-5))
            {
                return _cachedToken.AccessToken;
            }

            // 准备请求数据
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret),
                new KeyValuePair<string, string>("scope", _scope)
            });

            // 发送请求
            var response = await _httpClient.PostAsync(_tokenEndpoint, content);
            response.EnsureSuccessStatusCode();

            // 解析响应
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

            if (tokenResponse == null)
            {
                throw new InvalidOperationException("获取令牌失败：无法解析令牌响应");
            }

            // 缓存令牌
            _cachedToken = tokenResponse;
            _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

            return tokenResponse.AccessToken;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"获取令牌失败：{ex.Message}", ex);
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// 获取带有授权头的HTTP客户端
    /// </summary>
    /// <param name="baseAddress">API基础地址</param>
    /// <param name="forceRefresh">是否强制刷新令牌</param>
    /// <returns>HTTP客户端</returns>
    public async Task<HttpClient> GetAuthorizedHttpClientAsync(string baseAddress, bool forceRefresh = false)
    {
        var token = await GetAccessTokenAsync(forceRefresh);
        var client = new HttpClient
        {
            BaseAddress = new Uri(baseAddress)
        };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
    
    /// <summary>
    /// 为指定的HTTP客户端设置授权头
    /// </summary>
    /// <param name="client">HTTP客户端</param>
    /// <param name="forceRefresh">是否强制刷新令牌</param>
    /// <returns>HTTP客户端</returns>
    public async Task<HttpClient> SetAuthorizationHeaderAsync(HttpClient client, bool forceRefresh = false)
    {
        var token = await GetAccessTokenAsync(forceRefresh);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}

/// <summary>
/// 令牌响应
/// </summary>
public class TokenResponse
{
    /// <summary>
    /// 访问令牌
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    
    /// <summary>
    /// 令牌类型
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;
    
    /// <summary>
    /// 有效期（秒）
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
} 