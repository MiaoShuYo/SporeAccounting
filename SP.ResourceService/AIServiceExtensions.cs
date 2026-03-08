using SP.ResourceService.Models.Config;
using SP.ResourceService.Service;
using SP.ResourceService.Service.Impl;

namespace SP.ResourceService;

/// <summary>
/// DeepSeek服务扩展
/// </summary>
public static class AIServiceExtensions
{
    /// <summary>
    /// 添加AI服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddDeepSeekService(this IServiceCollection services, IConfiguration configuration, string sectionName = "DeepSeek")
    {
        services.Configure<DeepSeekOptions>(configuration.GetSection(sectionName));
        services.AddScoped<IAssistantService, DeepSeekAssistantServiceImpl>();
        return services;
    }
}