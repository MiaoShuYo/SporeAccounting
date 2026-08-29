namespace SP.FinanceService.Score;

/// <summary>
/// LLM 驱动的财务画像数据，承载多维度的原始财务信息
/// </summary>
public class FinancialPortraitData
{
    /// <summary>
    /// 账本 ID
    /// </summary>
    public long AccountBookId { get; set; }

    /// <summary>
    /// 账本名称
    /// </summary>
    public string AccountBookName { get; set; } = string.Empty;

    /// <summary>
    /// 用户拥有的账本总数（用于 LLM 理解数据范围；>1 时本画像仅为部分财务数据）
    /// </summary>
    public int AccountBookCount { get; set; } = 1;

    /// <summary>
    /// 是否为全量聚合视图（true=用户所有账本汇总，false=单个账本）
    /// </summary>
    public bool IsAggregate { get; set; }

    /// <summary>
    /// 统计周期开始日期
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// 统计周期结束日期
    /// </summary>
    public DateTime PeriodEnd { get; set; }

    /// <summary>
    /// 当月总收入
    /// </summary>
    public decimal MonthlyIncome { get; set; }

    /// <summary>
    /// 当月总支出
    /// </summary>
    public decimal MonthlyExpense { get; set; }

    /// <summary>
    /// 近三个月月均收入
    /// </summary>
    public decimal AvgMonthlyIncome3M { get; set; }

    /// <summary>
    /// 近三个月月均支出
    /// </summary>
    public decimal AvgMonthlyExpense3M { get; set; }

    /// <summary>
    /// 当月储蓄金额
    /// </summary>
    public decimal MonthlySavings => MonthlyIncome - MonthlyExpense;

    /// <summary>
    /// 当月储蓄率
    /// </summary>
    public decimal SavingsRate => MonthlyIncome > 0 ? MonthlySavings / MonthlyIncome : 0;

    /// <summary>
    /// 近三个月储蓄率趋势：每个月的 (年, 月, 储蓄率)
    /// </summary>
    public List<MonthlySavingsTrend> SavingsTrend3M { get; set; } = new();

    /// <summary>
    /// 当月各支出分类明细
    /// </summary>
    public List<CategoryExpenseDetail> CategoryDetails { get; set; } = new();

    /// <summary>
    /// 当月预算执行情况
    /// </summary>
    public List<BudgetExecutionDetail> BudgetExecution { get; set; } = new();

    /// <summary>
    /// 近三个月各月收入列表
    /// </summary>
    public List<MonthlyIncomeItem> MonthlyIncomes3M { get; set; } = new();

    /// <summary>
    /// 收入波动系数 (CV = 标准差 / 均值)
    /// </summary>
    public decimal? IncomeCV { get; set; }

    /// <summary>
    /// 最近一次历史建议内容（可选）
    /// </summary>
    public string? LastSuggestionContent { get; set; }

    /// <summary>
    /// 最近一次健康评分（可选）
    /// </summary>
    public decimal? LastTotalScore { get; set; }
}