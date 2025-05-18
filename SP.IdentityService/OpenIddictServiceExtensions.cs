using Microsoft.IdentityModel.Tokens;
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
                options.SetTokenEndpointUris("connect/token");

                // 启用密码模式
                options.AllowPasswordFlow() // 开启密码模式
                    .AllowClientCredentialsFlow()
                    .AllowRefreshTokenFlow(); // 开启刷新令牌

                options.RegisterScopes("api");
                // 使用开发环境下的临时密钥（生产环境请使用持久化证书）
                options.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

                options.AddSigningKey(
                    new SymmetricSecurityKey(
                        Convert.FromBase64String(signingKey)));
                options.AddEncryptionKey(
                    new SymmetricSecurityKey(Convert.FromBase64String(encryptionKey)));
                        
                // 允许接收表单数据
                options.AcceptAnonymousClients();
                
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
