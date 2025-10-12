using SP.MLService.Models.Dto;

namespace SP.MLService.Models.Request;

/// <summary>
/// 预测请求DTO
/// </summary>
/// <remarks>
/// 包含进行类目预测所需的全部信息
/// </remarks>
public class PredictionRequest
{
    /// <summary>
    /// 消费描述文本（如"星巴克咖啡"、"地铁卡充值"）
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// 用户的全部自定义类目列表
    /// </summary>
    public List<CategoryDto> Categories { get; set; } = new();

    /// <summary>
    /// 用户标识（用于个性化预测）
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 商户信息（可选，如"美团外卖"、"支付宝"）
    /// </summary>
    public string Merchant { get; set; } = string.Empty;

    /// <summary>
    /// 金额分箱（0-4，用于金额相关的特征）
    /// </summary>
    public float AmountBucket { get; set; }

    /// <summary>
    /// 消费时间（0-23小时，用于时间相关的特征）
    /// </summary>
    public float HourOfDay { get; set; }
}