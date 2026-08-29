using System.Text;
using SP.ReportService.Models.Response;

namespace SP.ReportService.Insights;

/// <summary>
/// 报表智能解读 Prompt 构建器
/// </summary>
public static class ReportInsightPromptBuilder
{
    /// <summary>
    /// 构建报表智能解读 Prompt
    /// </summary>
    public static string Build(ReportInsightPortrait portrait, ReportInsightResponse ruleResult)
    {
        var sb = new StringBuilder();

        sb.AppendLine("你是一位个人财务助手，擅长把报表数据解释成普通用户能理解、愿意看、能行动的内容。");
        sb.AppendLine("你的表达要温和、清晰、具体。不要夸大风险，也不要对用户的消费选择做道德评价。");
        sb.AppendLine();

        sb.AppendLine("【数据范围】");
        sb.AppendLine($"- 解读类型：{portrait.InsightType}");
        sb.AppendLine($"- 统计周期：{portrait.DataPeriod}");
        sb.AppendLine($"- 是否有可分析数据：{portrait.HasData}");
        sb.AppendLine();

        sb.AppendLine("【收支数据】");
        sb.AppendLine($"- 本期收入：{portrait.Income:F2} 元");
        sb.AppendLine($"- 本期支出：{portrait.Expense:F2} 元");
        sb.AppendLine($"- 本期结余：{portrait.Balance:F2} 元");
        sb.AppendLine($"- 上期收入：{portrait.PreviousIncome:F2} 元");
        sb.AppendLine($"- 上期支出：{portrait.PreviousExpense:F2} 元");
        sb.AppendLine($"- 上期结余：{portrait.PreviousBalance:F2} 元");
        sb.AppendLine($"- 收入变化率：{portrait.IncomeChangeRate:P1}");
        sb.AppendLine($"- 支出变化率：{portrait.ExpenseChangeRate:P1}");
        sb.AppendLine($"- 结余变化额：{portrait.BalanceChange:F2} 元");
        sb.AppendLine();

        sb.AppendLine("【预算数据】");
        sb.AppendLine($"- 预算总额：{portrait.BudgetTotalAmount:F2} 元");
        sb.AppendLine($"- 已用预算：{portrait.BudgetUsedAmount:F2} 元");
        sb.AppendLine($"- 剩余预算：{portrait.BudgetRemaining:F2} 元");
        sb.AppendLine($"- 预算使用率：{portrait.BudgetUsageRate:P1}");
        sb.AppendLine($"- 趋势变化率：{portrait.TrendChangeRate:P1}");
        sb.AppendLine();

        sb.AppendLine("【分类数据】");
        if (portrait.Categories.Count == 0)
        {
            sb.AppendLine("- 暂无分类数据");
        }
        else
        {
            foreach (var category in portrait.Categories.Take(10))
            {
                sb.AppendLine($"- {category.CategoryName}：金额 {category.Amount:F2} 元，占比 {category.Percentage:P1}，预算 {category.BudgetAmount:F2} 元，已用 {category.UsedAmount:F2} 元，使用率 {category.UsageRate:P1}");
            }
        }
        sb.AppendLine();

        sb.AppendLine("【规则引擎初步结论】");
        sb.AppendLine($"- 健康等级：{ruleResult.HealthLevel}");
        sb.AppendLine($"- 概括：{ruleResult.Summary}");
        foreach (var risk in ruleResult.Risks)
        {
            sb.AppendLine($"- 风险：{risk.Title}，{risk.Content}");
        }
        foreach (var suggestion in ruleResult.Suggestions)
        {
            sb.AppendLine($"- 建议：{suggestion.Title}，{suggestion.Content}");
        }
        sb.AppendLine();

        sb.AppendLine("请返回一个 JSON 对象，字段必须匹配 ReportInsightResponse：");
        sb.AppendLine("- InsightType：沿用输入解读类型");
        sb.AppendLine("- DataPeriod：沿用输入统计周期");
        sb.AppendLine("- Summary：一句话概括本期状态，控制在 80 字以内");
        sb.AppendLine("- HealthLevel：只能使用 Healthy、Attention、Risk、Overrun、NoData");
        sb.AppendLine("- GeneratedBy：必须返回 LLM");
        sb.AppendLine("- GeneratedAt：返回当前时间或可解析时间");
        sb.AppendLine("- Metrics：保留核心指标，不要编造数字");
        sb.AppendLine("- Highlights：最多 3 条亮点");
        sb.AppendLine("- Risks：最多 5 条风险");
        sb.AppendLine("- Suggestions：最多 5 条行动建议");
        sb.AppendLine();

        sb.AppendLine("【安全边界】");
        sb.AppendLine("- 不要给出具体投资建议");
        sb.AppendLine("- 不要推荐借贷平台、金融产品或具体证券");
        sb.AppendLine("- 不要评价用户消费选择的道德对错");
        sb.AppendLine("- 只能围绕预算管理、支出复盘、记录习惯和现金流改善给建议");

        return sb.ToString();
    }
}