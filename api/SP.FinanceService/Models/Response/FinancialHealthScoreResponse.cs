namespace SP.FinanceService.Models.Response;

/// <summary>
/// 财务健康评分响应模型
/// </summary>
public class FinancialHealthScoreResponse
{
    /// <summary>
    /// 记录 ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 账本 ID
    /// </summary>
    public long AccountBookId { get; set; }

    /// <summary>
    /// 总评分
    /// </summary>
    public decimal TotalScore { get; set; }

    /// <summary>
    /// 收支比率得分
    /// </summary>
    public decimal IncomeExpenseRatioScore { get; set; }

    /// <summary>
    /// 储蓄率得分
    /// </summary>
    public decimal SavingsRateScore { get; set; }

    /// <summary>
    /// 预算执行率得分（无预算数据时为 null）
    /// </summary>
    public decimal? BudgetComplianceScore { get; set; }

    /// <summary>
    /// 收入稳定性得分
    /// </summary>
    public decimal IncomeStabilityScore { get; set; }

    /// <summary>
    /// 健康等级（0=较差, 1=一般, 2=良好, 3=优秀）
    /// </summary>
    public int HealthLevel { get; set; }

    /// <summary>
    /// 健康等级名称
    /// </summary>
    public string HealthLevelName { get; set; } = string.Empty;

    /// <summary>
    /// 统计周期开始日期
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// 统计周期结束日期
    /// </summary>
    public DateTime PeriodEnd { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateDateTime { get; set; }
}
