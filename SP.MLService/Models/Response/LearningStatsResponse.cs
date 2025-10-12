namespace SP.MLService.Models.Response;

/// <summary>
/// 学习统计响应DTO
/// </summary>
/// <remarks>
/// 包含渐进式学习的各项统计指标
/// </remarks>
public class LearningStatsResponse
{
    /// <summary>
    /// 总反馈数（用户选择+纠正的总次数）
    /// </summary>
    public int TotalFeedbacks { get; set; }

    /// <summary>
    /// 训练数据量（转换为LTR格式的样本数）
    /// </summary>
    public int TrainingDataSize { get; set; }

    /// <summary>
    /// 模型是否存在（是否已完成至少一次训练）
    /// </summary>
    public bool ModelExists { get; set; }

    /// <summary>
    /// 近期反馈数（最近10条反馈的数量）
    /// </summary>
    public int RecentFeedbacks { get; set; }
}