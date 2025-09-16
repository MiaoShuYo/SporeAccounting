using SP.ResourceService.Models.Config;
using SP.ResourceService.Service;
using SP.ResourceService.Service.Impl;

namespace SP.ResourceService;

public static class OCRServiceExtensions
{
    public static IServiceCollection AddOCRService(this IServiceCollection services, IConfiguration configuration, string sectionName = "BaiduOCR")
    {
        services.Configure<BaiduOCROptions>(configuration.GetSection(sectionName));
        services.AddScoped<IOCRService, BaiduOCRServiceImpl>();
        return services;
    }
}