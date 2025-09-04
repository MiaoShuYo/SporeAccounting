using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SP.Common.Message.SmS.Model;
using SP.Common.Message.SmS.Services;
using SP.Common.Message.SmS.Services.Impl;

namespace SP.Common.Message.SmS;

/// <summary>
/// Twilio 短信服务扩展类
/// </summary>
public static class TwilioSmSExtensions
{
    /// <summary>
    /// 添加Twilio短信服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddTwilioSmSService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TwilioSmsOptions>(configuration.GetSection("Twilio"));
        services.AddScoped<ISmSService, TwilioSmSServiceImpl>();
        return services;
    }
}