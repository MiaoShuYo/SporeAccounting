using SP.MLService.Models.Enumeration;

namespace SP.MLService.Models.Response;

/// <summary>
/// AI使用记录响应
/// </summary>
public class AIUsageRecordResponse
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 使用类型
    /// </summary>
    public AIUsageRecordTypeEnum UsageType { get; set; }
}