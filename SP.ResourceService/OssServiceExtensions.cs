using SP.ResourceService.Models.Config;
using SP.ResourceService.Service;
using SP.ResourceService.Service.Impl;

namespace SP.ResourceService;

/// <summary>
/// 对象存储服务扩展
/// </summary>
public static class OssServiceExtensions
{
    /// <summary>
    /// 添加对象存储服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddOssService(this IServiceCollection services, IConfiguration configuration, string sectionName = "Minio")
    {
        services.Configure<MinioOptions>(configuration.GetSection(sectionName));
        services.AddScoped<IOssService, MinioOssServiceImpl>();
        return services;
    }
}