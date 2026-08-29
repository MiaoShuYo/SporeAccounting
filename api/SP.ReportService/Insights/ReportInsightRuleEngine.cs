using SP.ReportService.Models.Response;

namespace SP.ReportService.Insights;

/// <summary>
/// 报表智能解读规则引擎
/// </summary>
public class ReportInsightRuleEngine
{
    private const decimal AttentionBudgetUsageRate = 0.70m;
    private const decimal RiskBudgetUsageRate = 0.90m;
    private const decimal OverrunBudgetUsageRate = 1.00m;
    private const decimal HighCategoryPercentage = 0.40m;
    private const decimal ExpenseGrowthWarningRate = 0.20m;

    /// <summary>
    /// 根据画像生成规则解读
    /// </summary>
    public ReportInsightResponse Generate(ReportInsightPortrait portrait)
    {
        return portrait.InsightType == "Budget"
            ? GenerateBudgetInsight(portrait)
            : GenerateMonthlyReportInsight(portrait);
    }

    private static ReportInsightResponse CreateBaseResponse(ReportInsightPortrait portrait)
    {
        return new ReportInsightResponse
        {
            InsightType = portrait.InsightType,
            DataPeriod = portrait.DataPeriod,
            GeneratedBy = "Rule",
            GeneratedAt = DateTime.Now
        };
    }

    private static ReportInsightResponse GenerateMonthlyReportInsight(ReportInsightPortrait portrait)
    {
        var response = CreateBaseResponse(portrait);
        response.Metrics.AddRange([
            new ReportInsightMetricResponse { Name = "收入", Value = portrait.Income, Unit = "元", Description = "本期识别到的收入总额" },
            new ReportInsightMetricResponse { Name = "支出", Value = portrait.Expense, Unit = "元", Description = "本期识别到的支出总额" },
            new ReportInsightMetricResponse { Name = "结余", Value = portrait.Balance, Unit = "元", Description = "收入减支出后的结余" }
        ]);

        if (!portrait.HasData)
        {
            response.HealthLevel = "NoData";
            response.Summary = "当前周期暂无足够的报表数据，暂时无法判断财务变化。";
            response.Suggestions.Add(CreateItem("数据完整性", "补充记录", "建议先补全本期收入、支出或分类数据，系统才能给出更准确的财务解读。", "Low"));
            return response;
        }

        if (portrait.Balance < 0)
        {
            response.HealthLevel = "Risk";
            response.Risks.Add(CreateItem("收支结余", "本期出现负结余", $"本期结余为 {portrait.Balance:F2} 元，支出已经高于收入，需要优先关注现金流压力。", "High"));
            response.Suggestions.Add(CreateItem("收支结余", "先稳住必要支出", "建议先区分必要支出和弹性支出，优先压缩可延后的消费，让下个周期的结余回到正数。", "High"));
        }
        else
        {
            response.HealthLevel = "Healthy";
            response.Highlights.Add(CreateItem("收支结余", "本期保持正结余", $"本期结余为 {portrait.Balance:F2} 元，整体现金流处于较稳的状态。", "Info"));
        }

        if (portrait.ExpenseChangeRate > ExpenseGrowthWarningRate)
        {
            response.HealthLevel = response.HealthLevel == "Healthy" ? "Attention" : response.HealthLevel;
            response.Risks.Add(CreateItem("支出趋势", "支出增长较快", $"本期支出较上期增长 {portrait.ExpenseChangeRate:P1}，建议检查是否存在临时性大额支出或习惯性消费上升。", "Medium"));
        }

        if (portrait.BalanceChange < 0)
        {
            response.Risks.Add(CreateItem("财务变化", "结余较上期下降", $"本期结余较上期减少 {Math.Abs(portrait.BalanceChange):F2} 元，财务状态相比上期有所走弱。", "Medium"));
        }

        AddCategoryInsights(response, portrait);

        if (response.Suggestions.Count == 0)
        {
            response.Suggestions.Add(CreateItem("整体", "保持当前节奏", "当前周期没有明显异常，可以继续保持记录习惯，并关注占比较高的分类是否持续上升。", "Low"));
        }

        response.Summary = response.HealthLevel switch
        {
            "Risk" => "本期财务状态存在明显压力，重点需要关注负结余或支出过快增长。",
            "Attention" => "本期整体可控，但支出变化已经出现需要留意的信号。",
            _ => "本期财务状态整体平稳，结余和分类支出没有出现明显失控信号。"
        };

        return response;
    }

    private static ReportInsightResponse GenerateBudgetInsight(ReportInsightPortrait portrait)
    {
        var response = CreateBaseResponse(portrait);
        response.Metrics.AddRange([
            new ReportInsightMetricResponse { Name = "预算总额", Value = portrait.BudgetTotalAmount, Unit = "元", Description = "当前预算周期内的预算总额" },
            new ReportInsightMetricResponse { Name = "预算已用", Value = portrait.BudgetUsedAmount, Unit = "元", Description = "当前预算周期内已消耗的预算" },
            new ReportInsightMetricResponse { Name = "预算使用率", Value = portrait.BudgetUsageRate * 100, Unit = "%", Description = "预算已用金额占预算总额的比例" }
        ]);

        if (!portrait.HasData || portrait.BudgetTotalAmount <= 0)
        {
            response.HealthLevel = "NoData";
            response.Summary = "当前暂无可分析的预算数据，暂时无法判断预算执行状态。";
            response.Suggestions.Add(CreateItem("预算管理", "先建立预算基线", "建议先为主要支出分类设置预算，系统才能持续判断预算是否健康。", "Low"));
            return response;
        }

        response.HealthLevel = portrait.BudgetUsageRate switch
        {
            > OverrunBudgetUsageRate => "Overrun",
            >= RiskBudgetUsageRate => "Risk",
            >= AttentionBudgetUsageRate => "Attention",
            _ => "Healthy"
        };

        if (portrait.BudgetUsageRate > OverrunBudgetUsageRate)
        {
            response.Risks.Add(CreateItem("预算执行", "综合预算已经超支", $"当前预算使用率为 {portrait.BudgetUsageRate:P1}，已超出预算上限，需要尽快收紧后续支出。", "High"));
        }
        else if (portrait.BudgetUsageRate >= RiskBudgetUsageRate)
        {
            response.Risks.Add(CreateItem("预算执行", "预算接近用尽", $"当前预算使用率为 {portrait.BudgetUsageRate:P1}，剩余额度已经不多，后续消费需要谨慎。", "High"));
        }
        else if (portrait.BudgetUsageRate >= AttentionBudgetUsageRate)
        {
            response.Risks.Add(CreateItem("预算执行", "预算消耗偏快", $"当前预算使用率为 {portrait.BudgetUsageRate:P1}，建议留意接下来几天的消费节奏。", "Medium"));
        }
        else
        {
            response.Highlights.Add(CreateItem("预算执行", "预算进度健康", $"当前预算使用率为 {portrait.BudgetUsageRate:P1}，整体仍处在相对健康的区间。", "Info"));
        }

        if (portrait.TrendChangeRate > ExpenseGrowthWarningRate)
        {
            response.Risks.Add(CreateItem("预算趋势", "近期预算消耗加速", $"最新趋势点的预算消耗较上一节点增长 {portrait.TrendChangeRate:P1}，可能存在集中消费。", "Medium"));
        }

        AddBudgetCategoryInsights(response, portrait);

        if (response.Suggestions.Count == 0)
        {
            response.Suggestions.Add(CreateItem("预算管理", "保持预算节奏", "当前预算执行整体可控，建议继续关注高占比分类，避免月底集中超支。", "Low"));
        }

        response.Summary = response.HealthLevel switch
        {
            "Overrun" => "当前预算已经超支，需要优先处理超支分类并控制后续消费。",
            "Risk" => "当前预算接近用尽，预算执行处于危险区间。",
            "Attention" => "当前预算消耗偏快，但仍有调整空间。",
            _ => "当前预算执行整体健康，暂未发现明显超支风险。"
        };

        return response;
    }

    private static void AddCategoryInsights(ReportInsightResponse response, ReportInsightPortrait portrait)
    {
        foreach (var category in portrait.Categories.OrderByDescending(c => c.Percentage).Take(3))
        {
            response.Highlights.Add(CreateItem("分类支出", category.CategoryName, $"{category.CategoryName} 本期金额为 {category.Amount:F2} 元，占比 {category.Percentage:P1}。", "Info"));

            if (category.Percentage >= HighCategoryPercentage)
            {
                response.HealthLevel = response.HealthLevel == "Healthy" ? "Attention" : response.HealthLevel;
                response.Risks.Add(CreateItem("分类支出", $"{category.CategoryName} 占比较高", $"{category.CategoryName} 已占本期支出的 {category.Percentage:P1}，如果这不是一次性支出，建议重点复盘。", "Medium"));
                response.Suggestions.Add(CreateItem("分类支出", $"复盘 {category.CategoryName}", $"可以查看 {category.CategoryName} 下的具体记录，把可延后或可替代的支出先标出来。", "Medium"));
            }
        }
    }

    private static void AddBudgetCategoryInsights(ReportInsightResponse response, ReportInsightPortrait portrait)
    {
        foreach (var category in portrait.Categories.Where(c => c.BudgetAmount > 0).OrderByDescending(c => c.UsageRate).Take(5))
        {
            if (category.UsageRate > OverrunBudgetUsageRate)
            {
                response.Risks.Add(CreateItem("分类预算", $"{category.CategoryName} 已超支", $"{category.CategoryName} 已使用 {category.UsedAmount:F2} 元，超过预算 {category.BudgetAmount:F2} 元。", "High"));
                response.Suggestions.Add(CreateItem("分类预算", $"控制 {category.CategoryName}", $"建议本周期剩余时间优先控制 {category.CategoryName}，避免综合预算继续扩大缺口。", "High"));
            }
            else if (category.UsageRate >= RiskBudgetUsageRate)
            {
                response.Risks.Add(CreateItem("分类预算", $"{category.CategoryName} 接近用尽", $"{category.CategoryName} 预算使用率已达 {category.UsageRate:P1}，剩余额度为 {category.Remaining:F2} 元。", "High"));
            }
        }
    }

    private static ReportInsightItemResponse CreateItem(string dimension, string title, string content, string severity)
    {
        return new ReportInsightItemResponse
        {
            Dimension = dimension,
            Title = title,
            Content = content,
            Severity = severity
        };
    }
}