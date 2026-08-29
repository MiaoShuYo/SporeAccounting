using AutoMapper;
using SP.MLService.Models.Entity;
using SP.MLService.Models.Response;
using SP.MLService.Models.Request;

namespace SP.MLService;

/// <summary>
/// AI使用记录映射配置
/// </summary>
public class MLProfile : Profile
{
    /// <summary>
    /// 构造函数，配置映射关系
    /// </summary>
    public MLProfile()
    {

        // 配置从 AIUsageRecord 到 AIUsageRecordResponse 的映射
        CreateMap<AIUsageRecord, AIUsageRecordResponse>();

        // 配置从 AIUsageRecordRequestAdd 到 AIUsageRecord 的映射
        CreateMap<AIUsageRecordRequestAdd, AIUsageRecord>();
    }
}