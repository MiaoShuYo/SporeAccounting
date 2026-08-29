using Microsoft.Extensions.Configuration;

namespace SP.Common.ConfigService;

/// <summary>
/// Jwt配置服务
/// </summary>
public class JwtConfigService
{
    private readonly IConfiguration _configuration;

    public JwtConfigService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetJwtSecret()
    {
        // 假设配置中心的key为Jwt:SigningKey
        return _configuration["Jwt:SigningKey"] ?? string.Empty;
    }
} 