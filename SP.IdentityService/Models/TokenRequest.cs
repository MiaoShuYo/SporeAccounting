using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SP.IdentityService.Models;

/// <summary>
/// 令牌请求模型
/// </summary>
public class TokenRequest
{
    /// <summary>
    /// 授权类型
    /// </summary>
    [Required]
    [JsonPropertyName("grant_type")]
    public string GrantType { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 授权范围
    /// </summary>
    [JsonPropertyName("scope")]
    public string Scope { get; set; } = string.Empty;

    /// <summary>
    /// 客户端ID
    /// </summary>
    [JsonPropertyName("client_id")]
    public string ClientId { get; set; } = string.Empty;
    
    /// <summary>
    /// 刷新令牌
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
}