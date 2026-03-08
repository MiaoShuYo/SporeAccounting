using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Score;

/// <summary>
/// 财务改善建议生成引擎
/// </summary>
public static class SuggestionEngine
{
    /// <summary>
    /// 根据各维度得分与原始数据生成改善建议
    /// </summary>
    public static List<FinancialSuggestionResponse> Generate(
        decimal income,
        decimal expense,
        decimal incomeExpenseRatioScore,
        decimal savingsRateScore,
        decimal? budgetComplianceScore,
        decimal incomeStabilityScore)
    {
        var suggestions = new List<FinancialSuggestionResponse>();

        AddIncomeExpenseSuggestions(suggestions, income, expense, incomeExpenseRatioScore);
        AddSavingsRateSuggestions(suggestions, income, expense, savingsRateScore);
        AddBudgetComplianceSuggestions(suggestions, budgetComplianceScore);
        AddIncomeStabilitySuggestions(suggestions, incomeStabilityScore);

        if (suggestions.Count == 0)
        {
            suggestions.Add(new FinancialSuggestionResponse
            {
                Dimension = "整体",
                Score = 100,
                Suggestion = "您的财务状况良好！建议继续保持，并考虑通过理财投资实现资产增值",
                Priority = "Low"
            });
        }

        return suggestions;
    }

    private static void AddIncomeExpenseSuggestions(
        List<FinancialSuggestionResponse> suggestions,
        decimal income,
        decimal expense,
        decimal score)
    {
        if (income <= 0)
        {
            suggestions.Add(new FinancialSuggestionResponse
            {
                Dimension = "收支比率",
                Score = 0,
                Suggestion = "本期未记录收入，请及时补录收入数据以获得准确评分",
                Priority = "High"
            });
            return;
        }

        decimal ratio = expense / income;
        if (ratio > 1.0m)
        {
            suggestions.Add(new FinancialSuggestionResponse
            {
                Dimension = "收支比率",
                Score = score,
                Suggestion = $"您的支出（{expense:F2}）已超过收入（{income:F2}），建议立即削减非必要开销，避免负债",
                Priority = "High"
            });
        }
        else if (ratio > 0.9m)
        {
            suggestions.Add(new FinancialSuggestionResponse
            {
                Dimension = "收支比率",
                Score = score,
                Suggestion = "支出占收入 90% 以上，财务较为紧张，建议优化支出结构",
                Priority = "High"
            });
        }
        else if (ratio > 0.7m)
        {
            suggestions.Add(new FinancialSuggestionResponse
            {
                Dimension = "收支比率",
                Score = score,
                Suggestion = "支出占收入 70% 以上，建议适当压缩娱乐、餐饮等弹性支出",
                Priority = "Medium"
            });
        }
    }

    private static void AddSavingsRateSuggestions(
        List<FinancialSuggestionResponse> suggestions,
        decimal income,
        decimal expense,
        decimal score)
    {
        if (income <= 0) return;

        decimal savingsRate = (income - expense) / income;
        if (savingsRate < 0)
        {
            suggestions.Add(new FinancialSuggestionResponse
            {
                Dimension = "储蓄率",
                Score = score,
                Suggestion = "本期储蓄率为负，建议建立紧急备用金（3~6 个月生活费）并制定还款计划",
                Priority = "High"
            });
        }
        else if (savingsRate < 0.1m)
        {
            suggestions.Add(new FinancialSuggestionResponse
            {
                Dimension = "储蓄率",
                Score = score,
                Suggestion = $"储蓄率仅 {savingsRate:P0}，低于推荐的 10%，建议每月强制储蓄至少 10% 收入",
                Priority = "High"
            });
        }
        else if (savingsRate < 0.2m)
        {
            suggestions.Add(new FinancialSuggestionResponse
            {
                Dimension = "储蓄率",
                Score = score,
                Suggestion = $"储蓄率 {savingsRate:P0}，建议逐步提升至 20% 以上，可考虑设置自动转账定期储蓄",
                Priority = "Medium"
            });
        }
    }

    private static void AddBudgetComplianceSuggestions(
        List<FinancialSuggestionResponse> suggestions,
        decimal? budgetComplianceScore)
    {
        if (budgetComplianceScore.HasValue && budgetComplianceScore.Value < 60)
        {
            suggestions.Add(new FinancialSuggestionResponse
            {
                Dimension = "预算执行",
                Score = budgetComplianceScore.Value,
                Suggestion = "多个预算类别超出计划，建议每周回顾一次实际支出，及时调整消费行为",
                Priority = "Medium"
            });
        }
    }

    private static void AddIncomeStabilitySuggestions(
        List<FinancialSuggestionResponse> suggestions,
        decimal incomeStabilityScore)
    {
        if (incomeStabilityScore < 60)
        {
            suggestions.Add(new FinancialSuggestionResponse
            {
                Dimension = "收入稳定性",
                Score = incomeStabilityScore,
                Suggestion = "近期收入波动较大，建议建立 3~6 个月的应急储备金以应对收入不稳定风险",
                Priority = "Medium"
            });
        }
    }
}
