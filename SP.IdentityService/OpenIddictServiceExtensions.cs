using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using SP.IdentityService.DB;

namespace SP.IdentityService;

/// <summary>
/// OpenIddict服务扩展
/// </summary>
public static class OpenIddictServiceExtensions
{
    /// <summary>
    /// 添加OpenIddict服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddOpenIddict(this IServiceCollection services, IConfiguration configuration)
    {
        string signingKey = configuration["Jwt:SigningKey"];
        string encryptionKey = configuration["Jwt:EncryptionKey"];
        services.AddOpenIddict()
            .AddCore(options =>
            {
                // 使用 EntityFrameworkCore作为数据源
                options.UseEntityFrameworkCore()
                    .UseDbContext<IdentityServerDbContext>();
            })
            .AddServer(options =>
            {
                // 设置令牌端点
                options.SetTokenEndpointUris("api/auth/token");
                
                // 设置撤销端点
                options.SetRevocationEndpointUris("api/auth/revoke");

                // 启用密码模式
                options.AllowPasswordFlow() // 开启密码模式
                    .AllowClientCredentialsFlow() // 开启客户端令牌模式
                    .AllowRefreshTokenFlow(); // 开启刷新令牌

                // 注册授权范围
                options.RegisterScopes("api", OpenIddictConstants.Scopes.OfflineAccess);

                // 注册所有资源
                options.RegisterClaims(
                    OpenIddictConstants.Claims.Name,
                    OpenIddictConstants.Claims.Role,
                    OpenIddictConstants.Claims.Email);

                // 配置令牌属性
                options.SetAccessTokenLifetime(TimeSpan.FromMinutes(30));
                options.SetRefreshTokenLifetime(TimeSpan.FromDays(14));
                options.SetRefreshTokenReuseLeeway(TimeSpan.FromMinutes(2));

                // 使用开发环境下的临时密钥（生产环境请使用持久化证书）
                options.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

                options.AddSigningKey(
                    new SymmetricSecurityKey(
                        Convert.FromBase64String(signingKey)));
                options.AddEncryptionKey(
                    new SymmetricSecurityKey(Convert.FromBase64String(encryptionKey)));

                // 配置令牌选项 - 使用引用刷新令牌使令牌更短
                options.UseReferenceRefreshTokens();
                options.DisableAccessTokenEncryption();

                // 集成 ASP.NET Core
                options.UseAspNetCore()
                    .EnableTokenEndpointPassthrough()
                    .DisableTransportSecurityRequirement(); // 开发模式下禁用HTTPS
            })
            .AddValidation(options =>
            {
                // 使用本地服务器进行验证
                options.UseLocalServer();
                // 使用 AspNetCore 进行验证
                options.UseAspNetCore();
            });

        services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        return services;
    }
}