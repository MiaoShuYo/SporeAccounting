using SP.MLService.Models.Response;
using SP.MLService.Models.Request;

namespace SP.MLService.Services;

/// <summary>
/// AI使用记录服务接口
/// </summary>
public interface IAIUsageRecordServer
{
    /// <summary>
    /// 获取用户AI使用记录
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>AI使用记录</returns>
    Task<List<AIUsageRecordResponse>> GetAIUsageRecordAsync(long userId);

    /// <summary>
    /// 新增AI使用记录
    /// </summary>
    /// <param name="record">AI使用记录</param>
    /// <returns>新增结果</returns>
    Task<bool> AddAIUsageRecordAsync(AIUsageRecordRequestAdd record);
}