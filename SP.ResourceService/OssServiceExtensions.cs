using SP.ResourceService.Models.Config;
using SP.ResourceService.Service;
using SP.ResourceService.Service.Impl;

namespace SP.ResourceService;

public static class OssServiceExtensions
{
    public static IServiceCollection AddOssService(this IServiceCollection services, IConfiguration configuration, string sectionName = "Minio")
    {
        services.Configure<MinioOptions>(configuration.GetSection(sectionName));
        services.AddScoped<IOssService, MinioOssServiceImpl>();
        return services;
    }
}