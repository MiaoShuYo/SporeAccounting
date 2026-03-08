using SP.ResourceService.Models.Config;
using SP.ResourceService.Service;
using SP.ResourceService.Service.Impl;

namespace SP.ResourceService;

/// <summary>
/// OCR服务扩展
/// </summary>
public static class OCRServiceExtensions
{
    /// <summary>
    /// 添加OCR服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddOCRService(this IServiceCollection services, IConfiguration configuration, string sectionName = "BaiduOCR")
    {
        services.Configure<BaiduOCROptions>(configuration.GetSection(sectionName));
        services.AddScoped<IOCRService, BaiduOCRServiceImpl>();
        return services;
    }
}