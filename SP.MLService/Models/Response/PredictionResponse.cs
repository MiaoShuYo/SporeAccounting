using SP.MLService.Models.Dto;

namespace SP.MLService.Models.Response;

/// <summary>
/// 预测响应DTO
/// </summary>
/// <remarks>
/// 包含预测结果和相关的置信度、方法等信息
/// </remarks>
public class PredictionResponse
{
    /// <summary>
    /// 预测的最佳匹配类目
    /// </summary>
    public CategoryDto PredictedCategory { get; set; } = new();

    /// <summary>
    /// 预测置信度（0-1，越高越可信）
    /// </summary>
    public float Confidence { get; set; }

    /// <summary>
    /// 预测方法（"RuleBased" 或 "MachineLearning"）
    /// </summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// 是否需要用户确认（置信度低于阈值时为true）
    /// </summary>
    public bool RequiresUserConfirmation { get; set; }
}