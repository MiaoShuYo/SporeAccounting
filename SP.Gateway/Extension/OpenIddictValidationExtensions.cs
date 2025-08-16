using OpenIddict.Validation.AspNetCore;
using SP.Gateway.Services;
using SP.Gateway.Services.Impl;

namespace SP.Gateway.Extension;

/// <summary>
/// OpenIddict验证扩展
/// </summary>
public static class OpenIddictValidationExtensions
{
    /// <summary>
    /// 添加OpenIddict验证服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddOpenIddictValidation(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<INacosServiceDiscoveryService, NacosServiceDiscoveryService>();
        services.AddSingleton<IGatewayConfigService, NacosGatewayConfigService>();
        services.AddScoped<ITokenIntrospectionService, TokenIntrospectionService>();

        services.AddHttpClient("IdentityServiceHealthCheck", client => { client.Timeout = TimeSpan.FromSeconds(10); });

        services.AddHttpClient("TokenIntrospection", client => { client.Timeout = TimeSpan.FromSeconds(30); });

        services.AddOpenIddict()
            .AddValidation(options =>
            {
                // 获取动态Issuer URL
                var discoveryService =
                    services.BuildServiceProvider().GetRequiredService<INacosServiceDiscoveryService>();
                var issuerUrl = discoveryService.GetBestIdentityServiceUrlAsync().Result;
                if (string.IsNullOrEmpty(issuerUrl))
                {
                    throw new Exception("No healthy IdentityService instances found in Nacos.");
                }

                // 从配置服务获取客户端凭证
                var configService = services.BuildServiceProvider().GetRequiredService<IGatewayConfigService>();
                var config = configService.GetIdentityServiceConfigAsync().Result;

                // 使用内省（introspection）进行远程验证
                options.UseIntrospection()
                    .SetIssuer(new Uri(issuerUrl))
                    .SetClientId(config.ClientId)
                    .SetClientSecret(config.ClientSecret);

                // 启用ASP.NET Core集成
                options.UseAspNetCore();

                // 配置令牌验证参数
                options.Configure(options =>
                {
                    options.TokenValidationParameters.ValidateIssuer = true;
                    options.TokenValidationParameters.ValidAudience = "api"; // 设置受众
                    options.TokenValidationParameters.ValidateAudience = true;
                    options.TokenValidationParameters.ValidateLifetime = true;
                    options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                });
            });

        services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        services.AddAuthorization();

        return services;
    }
}