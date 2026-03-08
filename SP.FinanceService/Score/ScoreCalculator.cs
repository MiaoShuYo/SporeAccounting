using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Score;

/// <summary>
/// 财务健康评分计算引擎
/// </summary>
public static class ScoreCalculator
{
    /// <summary>
    /// 计算收支比率得分（权重 30%）
    /// <para>支出 / 收入 越低越好</para>
    /// </summary>
    public static decimal CalcIncomeExpenseRatioScore(decimal income, decimal expense)
    {
        if (income <= 0) return 0;
        decimal ratio = expense / income;
        if (ratio <= 0.5m) return 100;
        if (ratio <= 0.7m) return 80;
        if (ratio <= 0.9m) return 60;
        if (ratio <= 1.0m) return 40;
        return Math.Max(0, Math.Round(20 - (ratio - 1.0m) * 100, 2));
    }

    /// <summary>
    /// 计算储蓄率得分（权重 30%）
    /// <para>（收入 - 支出）/ 收入 越高越好</para>
    /// </summary>
    public static decimal CalcSavingsRateScore(decimal income, decimal expense)
    {
        if (income <= 0) return 0;
        decimal savingsRate = (income - expense) / income;
        if (savingsRate >= 0.3m) return 100;
        if (savingsRate >= 0.2m) return 80;
        if (savingsRate >= 0.1m) return 60;
        if (savingsRate >= 0m) return 40;
        return 0;
    }

    /// <summary>
    /// 计算预算执行率得分（权重 25%）
    /// </summary>
    /// <param name="budgetItems">预算项列表：(预算金额, 实际支出)</param>
    /// <returns>得分 0~100；无预算数据时返回 null</returns>
    public static decimal? CalcBudgetComplianceScore(List<(decimal Budget, decimal Actual)> budgetItems)
    {
        if (budgetItems == null || budgetItems.Count == 0) return null;

        decimal totalScore = 0;
        int validCount = 0;
        foreach (var item in budgetItems)
        {
            if (item.Budget <= 0) continue;
            validCount++;
            decimal overrunRate = (item.Actual - item.Budget) / item.Budget;
            decimal itemScore = overrunRate switch
            {
                <= 0m => 100,
                <= 0.1m => 80,
                <= 0.2m => 60,
                <= 0.5m => 40,
                _ => 20
            };
            totalScore += itemScore;
        }

        return validCount == 0 ? null : Math.Round(totalScore / validCount, 2);
    }

    /// <summary>
    /// 计算收入稳定性得分（权重 15%）
    /// <para>使用变异系数（CV = 标准差 / 均值），越小越稳定</para>
    /// </summary>
    /// <param name="monthlyIncomes">各月收入列表（至少 2 个月）</param>
    public static decimal CalcIncomeStabilityScore(List<decimal> monthlyIncomes)
    {
        if (monthlyIncomes == null || monthlyIncomes.Count < 2) return 80;
        decimal mean = monthlyIncomes.Average();
        if (mean <= 0) return 0;
        decimal variance = monthlyIncomes.Sum(x => (x - mean) * (x - mean)) / monthlyIncomes.Count;
        decimal stdDev = (decimal)Math.Sqrt((double)variance);
        decimal cv = stdDev / mean;
        if (cv <= 0.05m) return 100;
        if (cv <= 0.10m) return 80;
        if (cv <= 0.20m) return 60;
        if (cv <= 0.30m) return 40;
        return 20;
    }

    /// <summary>
    /// 汇总计算总得分与健康等级
    /// <para>无预算数据时，将预算权重均摊至其余维度</para>
    /// </summary>
    public static (decimal TotalScore, HealthLevelEnum Level) CalcTotalScore(
        decimal incomeExpenseScore,
        decimal savingsRateScore,
        decimal? budgetComplianceScore,
        decimal incomeStabilityScore)
    {
        decimal total = budgetComplianceScore.HasValue
            ? incomeExpenseScore * 0.30m
              + savingsRateScore * 0.30m
              + budgetComplianceScore.Value * 0.25m
              + incomeStabilityScore * 0.15m
            : incomeExpenseScore * 0.40m
              + savingsRateScore * 0.40m
              + incomeStabilityScore * 0.20m;

        total = Math.Round(total, 2);

        HealthLevelEnum level = total switch
        {
            >= 80 => HealthLevelEnum.Excellent,
            >= 60 => HealthLevelEnum.Good,
            >= 40 => HealthLevelEnum.Fair,
            _ => HealthLevelEnum.Poor
        };

        return (total, level);
    }
}
