namespace SP.ReportService.Insights;

/// <summary>
/// 报表智能解读画像数据
/// </summary>
public class ReportInsightPortrait
{
    /// <summary>
    /// 解读类型
    /// </summary>
    public string InsightType { get; set; } = string.Empty;

    /// <summary>
    /// 数据周期
    /// </summary>
    public string DataPeriod { get; set; } = string.Empty;

    /// <summary>
    /// 是否存在可分析数据
    /// </summary>
    public bool HasData { get; set; }

    /// <summary>
    /// 本期收入
    /// </summary>
    public decimal Income { get; set; }

    /// <summary>
    /// 本期支出
    /// </summary>
    public decimal Expense { get; set; }

    /// <summary>
    /// 本期结余
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// 上期收入
    /// </summary>
    public decimal PreviousIncome { get; set; }

    /// <summary>
    /// 上期支出
    /// </summary>
    public decimal PreviousExpense { get; set; }

    /// <summary>
    /// 上期结余
    /// </summary>
    public decimal PreviousBalance { get; set; }

    /// <summary>
    /// 收入变化率
    /// </summary>
    public decimal IncomeChangeRate { get; set; }

    /// <summary>
    /// 支出变化率
    /// </summary>
    public decimal ExpenseChangeRate { get; set; }

    /// <summary>
    /// 结余变化额
    /// </summary>
    public decimal BalanceChange { get; set; }

    /// <summary>
    /// 预算总额
    /// </summary>
    public decimal BudgetTotalAmount { get; set; }

    /// <summary>
    /// 预算已用金额
    /// </summary>
    public decimal BudgetUsedAmount { get; set; }

    /// <summary>
    /// 预算剩余金额
    /// </summary>
    public decimal BudgetRemaining { get; set; }

    /// <summary>
    /// 预算使用率
    /// </summary>
    public decimal BudgetUsageRate { get; set; }

    /// <summary>
    /// 最新趋势金额
    /// </summary>
    public decimal LatestTrendAmount { get; set; }

    /// <summary>
    /// 上一趋势金额
    /// </summary>
    public decimal PreviousTrendAmount { get; set; }

    /// <summary>
    /// 趋势变化率
    /// </summary>
    public decimal TrendChangeRate { get; set; }

    /// <summary>
    /// 分类指标
    /// </summary>
    public List<ReportInsightCategoryMetric> Categories { get; set; } = new();
}