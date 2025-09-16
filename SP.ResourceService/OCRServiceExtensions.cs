using SP.ResourceService.Models.Config;

namespace SP.ResourceService;

public static class OCRServiceExtensions
{
    public static IServiceCollection AddOCRService(this IServiceCollection services, IConfiguration configuration, string sectionName = "BaiduOCR")
    {
        services.Configure<BaiduOCROptions>(configuration.GetSection(sectionName));
        return services;
    }
}