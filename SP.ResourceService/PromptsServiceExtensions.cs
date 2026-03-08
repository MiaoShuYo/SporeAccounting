using SP.ResourceService.Models.Config;

namespace SP.ResourceService;

/// <summary>
/// 提示词服务扩展
/// </summary>
public static class PromptsServiceExtensions
{
    /// <summary>
    /// 添加提示词服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddPromptsService(this IServiceCollection services, IConfiguration configuration, string sectionName = "Prompts")
    {
        services.Configure<PromptsOptions>(configuration.GetSection(sectionName));
        return services;
    }
}