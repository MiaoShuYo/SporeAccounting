using SP.ResourceService.Models.Config;
using SP.ResourceService.Service;
using SP.ResourceService.Service.Impl;
using SP.ResourceService.Service.OCR;

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
    /// <returns></returns>
    public static IServiceCollection AddOCRService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LlmOptions>(configuration.GetSection("LLM"));
        services.Configure<OcrOptions>(configuration.GetSection("OCR"));
        services.AddHttpClient(OpenAiCompatibleOcrProvider.HttpClientName);
        services.AddScoped<IOCRService, OCRServiceImpl>();
        services.AddScoped<IOcrProvider, OpenAiCompatibleOcrProvider>();
        return services;
    }
}