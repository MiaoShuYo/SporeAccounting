using AutoMapper;
using SP.ConfigService.Models.Entity;
using SP.ConfigService.Models.Request;
using SP.ConfigService.Models.Response;

namespace SP.ConfigService;

/// <summary>
/// 用户配置映射配置
/// </summary>
public class ConfigProfile : Profile
{
    public ConfigProfile()
    {
        // 配置从 ConfigEditRequest 到 Config 的映射
        CreateMap<ConfigEditRequest, Config>();

        // 配置从 Config 到 ConfigResponse 的映射
        CreateMap<Config, ConfigResponse>();
    }
}