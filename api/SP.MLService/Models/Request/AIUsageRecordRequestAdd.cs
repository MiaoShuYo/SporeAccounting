using SP.MLService.Models.Enumeration;

namespace SP.MLService.Models.Request;

/// <summary>
/// 新增AI使用记录请求
/// </summary>
public class AIUsageRecordRequestAdd
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