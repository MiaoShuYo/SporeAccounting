using System.Text;

namespace SP.FinanceService.Score;

/// <summary>
/// 财务建议 Prompt 构建器 —— 将财务画像数据嵌入精心设计的 Prompt 模板
/// </summary>
public static class SuggestionPromptBuilder
{
    /// <summary>
    /// 根据财务画像数据构建完整的 Prompt
    /// </summary>
    /// <param name="portrait">财务画像数据</param>
    /// <returns>完整的 Prompt 字符串</returns>
    public static string Build(FinancialPortraitData portrait)
    {
        var sb = new StringBuilder();

        // ══════ 角色定位 ══════
        sb.AppendLine("你是一位经验丰富的个人财务管理顾问，擅长用通俗易懂的语言帮助普通用户理解自己的财务状况。");
        sb.AppendLine("你的风格是温和但专业，善于在指出问题的同时给予鼓励，而不是居高临下地批评。");
        sb.AppendLine("你给出的每一条建议都应该是具体的、可落地的，而不是空洞的口号。");
        sb.AppendLine();

        // ══════ 数据输入 ══════
        sb.AppendLine("下面是一位用户的财务画像数据，请仔细阅读后给出改善建议。");
        sb.AppendLine();

        // 【数据范围说明】— 多账本上下文
        sb.AppendLine("【数据范围说明】");
        sb.AppendLine($"- 账本名称：{portrait.AccountBookName}");
        if (portrait.IsAggregate)
        {
            sb.AppendLine($"- 📊 汇总视图：本数据涵盖了该用户全部 {portrait.AccountBookCount} 个账本的汇总数据，反映用户整体财务状况");
        }
        else if (portrait.AccountBookCount > 1)
        {
            sb.AppendLine($"- ⚠️ 重要提示：该用户共有 {portrait.AccountBookCount} 个账本，本次分析仅涵盖「{portrait.AccountBookName}」这一个账本的数据，不代表用户整体财务状况");
            sb.AppendLine($"- 因此，如果该账本的收入/支出较低，可能是因为用户将不同类别的收支分散到了多个账本中，请在建议中注意这一限定");
        }
        sb.AppendLine();

        // 【收入支出概况】
        sb.AppendLine("【收入支出概况】");
        sb.AppendLine($"- 统计周期：{portrait.PeriodStart:yyyy-MM-dd} 至 {portrait.PeriodEnd:yyyy-MM-dd}");
        sb.AppendLine($"- 当月总收入：{portrait.MonthlyIncome:F2} 元");
        sb.AppendLine($"- 当月总支出：{portrait.MonthlyExpense:F2} 元");
        sb.AppendLine($"- 当月结余：{portrait.MonthlySavings:F2} 元（储蓄率 {portrait.SavingsRate:P1}）");
        sb.AppendLine($"- 近三个月月均收入：{portrait.AvgMonthlyIncome3M:F2} 元");
        sb.AppendLine($"- 近三个月月均支出：{portrait.AvgMonthlyExpense3M:F2} 元");
        sb.AppendLine();

        // 【分类支出明细】
        if (portrait.CategoryDetails.Count > 0)
        {
            sb.AppendLine("【分类支出明细】");
            foreach (var detail in portrait.CategoryDetails)
            {
                sb.AppendLine($"- {detail.CategoryName}：{detail.Amount:F2} 元（占比 {detail.Percentage:P1}）");
            }
            sb.AppendLine();
        }
        else
        {
            sb.AppendLine("【分类支出明细】");
            sb.AppendLine("本月暂无支出记录。");
            sb.AppendLine();
        }

        // 【预算执行情况】
        if (portrait.BudgetExecution.Count > 0)
        {
            sb.AppendLine("【预算执行情况】");
            foreach (var exec in portrait.BudgetExecution)
            {
                string status = exec.OverrunRate > 0
                    ? $"超支 {exec.OverrunRate:P1}"
                    : $"节余 {Math.Abs(exec.OverrunRate):P1}";
                sb.AppendLine($"- {exec.CategoryName}：预算 {exec.BudgetAmount:F2} 元，实际 {exec.ActualAmount:F2} 元（{status}）");
            }
            sb.AppendLine();
        }
        else
        {
            sb.AppendLine("【预算执行情况】");
            sb.AppendLine("用户未设置预算或暂无预算数据。");
            sb.AppendLine();
        }

        // 【储蓄与趋势】
        sb.AppendLine("【储蓄与趋势】");
        if (portrait.SavingsTrend3M.Count > 0)
        {
            foreach (var trend in portrait.SavingsTrend3M)
            {
                string trendLabel = trend.SavingsRate >= 0 ? $"储蓄率 {trend.SavingsRate:P1}" : $"负储蓄率 {trend.SavingsRate:P1}";
                sb.AppendLine($"- {trend.Year}年{trend.Month}月：{trendLabel}");
            }
        }
        else
        {
            sb.AppendLine("暂无近三个月储蓄趋势数据。");
        }
        sb.AppendLine();

        // 【收入稳定性】
        sb.AppendLine("【收入稳定性】");
        if (portrait.MonthlyIncomes3M.Count > 0)
        {
            foreach (var item in portrait.MonthlyIncomes3M)
            {
                sb.AppendLine($"- {item.Year}年{item.Month}月收入：{item.Income:F2} 元");
            }
            if (portrait.IncomeCV.HasValue)
            {
                string stabilityDesc = portrait.IncomeCV.Value switch
                {
                    <= 0.05m => "非常稳定",
                    <= 0.10m => "较为稳定",
                    <= 0.20m => "有一定波动",
                    <= 0.30m => "波动较大",
                    _ => "波动剧烈"
                };
                sb.AppendLine($"- 收入波动系数 CV = {portrait.IncomeCV.Value:F4}（{stabilityDesc}）");
            }
        }
        else
        {
            sb.AppendLine("暂无近三个月收入数据。");
        }
        sb.AppendLine();

        // 【历史参考】
        if (portrait.LastTotalScore.HasValue || !string.IsNullOrWhiteSpace(portrait.LastSuggestionContent))
        {
            sb.AppendLine("【历史参考】");
            if (portrait.LastTotalScore.HasValue)
            {
                sb.AppendLine($"- 上次健康评分：{portrait.LastTotalScore:F1} 分");
            }
            sb.AppendLine();
        }

        // ══════ 输出格式约束 ══════
        sb.AppendLine("请以 JSON 数组格式返回你的建议，每条建议包含以下字段：");
        sb.AppendLine("- Dimension（string）：关联的评分维度，可选值：\"收支比率\"、\"储蓄率\"、\"预算执行\"、\"收入稳定性\"、\"整体\"");
        sb.AppendLine("- Score（number）：该维度的建议得分（0~100），基于你对该维度健康状况的判断");
        sb.AppendLine("- Suggestion（string）：具体的、可落地的改善建议，语气温和、有共情感。不要使用编号列表，用自然的段落语言表达");
        sb.AppendLine("- Priority（string）：优先级，\"High\"（需立即关注）、\"Medium\"（建议改善）、\"Low\"（锦上添花）");
        sb.AppendLine();
        sb.AppendLine("注意：");
        sb.AppendLine("- 如果某个维度状况良好，可以不生成该维度的建议（无需对每个维度都生成一条）");
        sb.AppendLine("- 如果整体财务状况良好，至少返回一条\"整体\"维度的鼓励性建议");
        sb.AppendLine("- 如果有严重问题（如储蓄率为负、严重超支），优先返回这些高优先级建议");
        sb.AppendLine();

        // ══════ 温度与共情指令 ══════
        sb.AppendLine("【表达风格要求】");
        sb.AppendLine("你的建议应该在指出问题的同时表达理解和共情。例如：");
        sb.AppendLine("- 如果用户超支严重，不要说\"您的支出偏高，建议控制消费\"，而应该说\"这个月您的购物支出比平时多了不少，双十一确实容易让人冲动消费，这完全可以理解。不过如果下个月能适当收紧这部分预算，整体的财务状况会更加健康。\"");
        sb.AppendLine("- 提到具体消费类别时，用用户数据中实际出现的分类名称，而不是泛泛的\"非必要支出\"");
        sb.AppendLine("- 如果用户是第一次收到某类建议，语气可以是\"温和提醒\"；如果问题持续存在，语气可以是\"关切但依然鼓励\"");
        sb.AppendLine();

        // ══════ 安全边界 ══════
        sb.AppendLine("【安全边界】");
        sb.AppendLine("- 不要给出任何具体的投资建议（如\"建议买入某只基金/股票\"）");
        sb.AppendLine("- 不要对用户的消费选择做道德评判（如\"你不应该花这么多钱在游戏上\"）");
        sb.AppendLine("- 只围绕财务规划、消费习惯、预算管理这些领域给出合理建议");
        sb.AppendLine("- 不要建议用户使用任何借贷平台或具体金融产品");

        return sb.ToString();
    }
}
