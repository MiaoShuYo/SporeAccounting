using SP.MLService.Models.Dto;

namespace SP.MLService.Models.Request;

/// <summary>
/// 用户纠正反馈请求DTO
/// </summary>
/// <remarks>
/// 用于记录用户纠正模型错误预测的负反馈+正反馈
/// </remarks>
public class CorrectionFeedbackRequest
{
    /// <summary>
    /// 原始查询文本
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// 模型错误预测的类目（负样本）
    /// </summary>
    public CategoryDto WrongCategory { get; set; } = new();

    /// <summary>
    /// 用户纠正的正确类目（正样本）
    /// </summary>
    public CategoryDto CorrectCategory { get; set; } = new();

    /// <summary>
    /// 当时可选的全部类目
    /// </summary>
    public List<CategoryDto> AvailableCategories { get; set; } = new();

    /// <summary>
    /// 用户标识
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 商户信息
    /// </summary>
    public string Merchant { get; set; } = string.Empty;

    /// <summary>
    /// 金额分箱
    /// </summary>
    public float AmountBucket { get; set; }

    /// <summary>
    /// 消费时间
    /// </summary>
    public float HourOfDay { get; set; }
}