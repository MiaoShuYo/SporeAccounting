using SP.ReportService.Models.Response;
using SP.ReportService.Models.Enumeration;
using SP.ReportService.DB;
using SP.ReportService.Insights;
using SP.ReportService.RefitClient;

namespace SP.ReportService.Service.Impl;

/// <summary>
/// 预算报表服务实现
/// </summary>
public class BudgetReportServerImpl : IBudgetReportServer
{
    /// <summary>
    /// 预算服务API
    /// </summary>
    private readonly IBudgetServiceApi _budgetServiceApi;

    /// <summary>
    /// 预算记录服务API
    /// </summary>
    private readonly IBudgetRecordServiceApi _budgetRecordServiceApi;

    /// <summary>
    /// 报表智能解读生成器
    /// </summary>
    private readonly ReportInsightGenerator _reportInsightGenerator;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="budgetServiceApi">预算服务API</param>
    /// <param name="budgetRecordServiceApi">预算记录服务API</param>
    /// <param name="reportInsightGenerator">报表智能解读生成器</param>
    public BudgetReportServerImpl(
        IBudgetServiceApi budgetServiceApi,
        IBudgetRecordServiceApi budgetRecordServiceApi,
        ReportInsightGenerator reportInsightGenerator)
    {
        _budgetServiceApi = budgetServiceApi;
        _budgetRecordServiceApi = budgetRecordServiceApi;
        _reportInsightGenerator = reportInsightGenerator;
    }

    /// <summary>
    /// 预算进度
    /// </summary>
    /// <returns>
    /// 预算报表列表包括：
    /// 1. 综合预算进度
    /// 2. 各类别预算进度
    /// </returns>
    public async Task<List<BudgetProgressReportResponse>> GetBudgetProgress()
    {
        var response = await _budgetServiceApi.GetCurrentBudgetsAsync();
        if (response.IsSuccessStatusCode && response.Content != null)
        {
            var budgets = response.Content;
            var budgetProgressReports = new List<BudgetProgressReportResponse>();

            foreach (var budget in budgets)
            {
                var report = new BudgetProgressReportResponse
                {
                    Period = budget.Period,
                    IsComprehensive = false,
                    CategoryName = budget.TransactionCategoryName,
                    TotalAmount = budget.Amount,
                    UsedAmount = budget.Amount - budget.Remaining,
                    Remaining = budget.Remaining
                };
                budgetProgressReports.Add(report);
            }

            // 添加综合预算进度
            var totalAmount = budgets.Sum(b => b.Amount);
            var totalRemaining = budgets.Sum(b => b.Remaining);
            var comprehensiveReport = new BudgetProgressReportResponse
            {
                IsComprehensive = true,
                TotalAmount = totalAmount,
                UsedAmount = totalAmount - totalRemaining,
                Remaining = totalRemaining
            };
            budgetProgressReports.Insert(0, comprehensiveReport);
            return budgetProgressReports;
        }

        return new List<BudgetProgressReportResponse>();
    }

    /// <summary>
    /// 预算消耗趋势报表
    /// </summary>
    /// <returns>
    /// 预算消耗趋势报表列表包括：
    /// 1. 综合预算消耗趋势
    /// 2. 各类别预算消耗趋势
    /// 年预算消耗趋势报表按月展示，月预算消耗趋势报表按日展示，季度预算消耗趋势报表按周展示
    /// </returns>
    public async Task<List<BudgetConsumptionTrendReportResponse>> GetBudgetConsumptionTrend()
    {
        // 1. 获取预算记录
        var result = await _budgetRecordServiceApi.GetBudgetRecordsByBudgetIdsAsync();
        if (result.IsSuccessStatusCode && result.Content != null)
        {
            var budgetRecords = result.Content;
            List<BudgetConsumptionTrendReportResponse> trendReports = new List<BudgetConsumptionTrendReportResponse>();

            // 2. 获取当前预算信息
            var budgetResponse = await _budgetServiceApi.GetCurrentBudgetsAsync();
            if (budgetResponse.IsSuccessStatusCode && budgetResponse.Content != null)
            {
                var budgets = budgetResponse.Content;

                // 3. 综合预算消耗趋势（年预算消耗趋势报表按月展示，月预算消耗趋势报表按日展示，季度预算消耗趋势报表按周展示）
                var comprehensiveTrend = CalculateComprehensiveTrend(budgetRecords, budgets);
                trendReports.AddRange(comprehensiveTrend);

                // 4. 各类别预算消耗趋势（年预算消耗趋势报表按月展示，月预算消耗趋势报表按日展示，季度预算消耗趋势报表按周展示）
                var categoryTrends = CalculateCategoryTrends(budgetRecords, budgets);
                trendReports.AddRange(categoryTrends);
            }

            return trendReports;
        }

        return new List<BudgetConsumptionTrendReportResponse>();
    }

    /// <summary>
    /// 获取预算报表智能解读
    /// </summary>
    /// <returns>预算报表智能解读</returns>
    public async Task<ReportInsightResponse> GetBudgetInsightAsync()
    {
        List<BudgetProgressReportResponse> progressReports = await GetBudgetProgress();
        List<BudgetConsumptionTrendReportResponse> trendReports = await GetBudgetConsumptionTrend();
        ReportInsightPortrait portrait = BuildBudgetPortrait(progressReports, trendReports);
        return await _reportInsightGenerator.GenerateAsync(portrait);
    }

    /// <summary>
    /// 计算综合预算消耗趋势
    /// </summary>
    /// <param name="budgetRecords">预算记录</param>
    /// <param name="budgets">预算信息</param>
    /// <returns>综合预算消耗趋势报表</returns>
    private List<BudgetConsumptionTrendReportResponse> CalculateComprehensiveTrend(
        Dictionary<long, List<BudgetRecordResponse>> budgetRecords,
        List<BudgetResponse> budgets)
    {
        var trendReports = new List<BudgetConsumptionTrendReportResponse>();

        // 按预算周期分组
        var budgetsByPeriod = budgets.GroupBy(b => b.Period);

        foreach (var periodGroup in budgetsByPeriod)
        {
            var period = periodGroup.Key;
            var periodBudgets = periodGroup.ToList();

            // 获取该周期所有预算的ID
            var budgetIds = periodBudgets.Select(b => b.Id).ToList();

            // 获取该周期所有预算记录
            var allRecords = budgetIds
                .Where(id => budgetRecords.ContainsKey(id))
                .SelectMany(id => budgetRecords[id])
                .ToList();

            if (!allRecords.Any()) continue;

            // 根据周期类型进行时间分组
            var groupedRecords = GroupRecordsByPeriod(allRecords, period);

            foreach (var group in groupedRecords)
            {
                var totalUsedAmount = group.Sum(r => r.UsedAmount);
                trendReports.Add(new BudgetConsumptionTrendReportResponse
                {
                    Period = period,
                    IsComprehensive = true,
                    CategoryName = "综合预算",
                    TimePoint = group.Key,
                    ConsumedAmount = totalUsedAmount
                });
            }
        }

        return trendReports;
    }

    /// <summary>
    /// 计算各类别预算消耗趋势
    /// </summary>
    /// <param name="budgetRecords">预算记录</param>
    /// <param name="budgets">预算信息</param>
    /// <returns>各类别预算消耗趋势报表</returns>
    private List<BudgetConsumptionTrendReportResponse> CalculateCategoryTrends(
        Dictionary<long, List<BudgetRecordResponse>> budgetRecords,
        List<BudgetResponse> budgets)
    {
        var trendReports = new List<BudgetConsumptionTrendReportResponse>();

        // 按预算周期和类别分组
        var budgetsByPeriodAndCategory =
            budgets.GroupBy(b => new { b.Period, b.TransactionCategoryId, b.TransactionCategoryName });

        foreach (var group in budgetsByPeriodAndCategory)
        {
            var period = group.Key.Period;
            var categoryId = group.Key.TransactionCategoryId;
            var categoryName = group.Key.TransactionCategoryName;
            var periodBudgets = group.ToList();

            // 获取该类别预算的ID
            var budgetIds = periodBudgets.Select(b => b.Id).ToList();

            // 获取该类别所有预算记录
            var categoryRecords = budgetIds
                .Where(id => budgetRecords.ContainsKey(id))
                .SelectMany(id => budgetRecords[id])
                .ToList();

            if (!categoryRecords.Any()) continue;

            // 根据周期类型进行时间分组
            var groupedRecords = GroupRecordsByPeriod(categoryRecords, period);

            foreach (var recordGroup in groupedRecords)
            {
                var totalUsedAmount = recordGroup.Sum(r => r.UsedAmount);
                trendReports.Add(new BudgetConsumptionTrendReportResponse
                {
                    Period = period,
                    IsComprehensive = false,
                    CategoryName = categoryName,
                    TimePoint = recordGroup.Key,
                    ConsumedAmount = totalUsedAmount
                });
            }
        }

        return trendReports;
    }

    /// <summary>
    /// 根据预算周期对记录进行时间分组
    /// </summary>
    /// <param name="records">预算记录</param>
    /// <param name="period">预算周期</param>
    /// <returns>分组后的记录</returns>
    private IEnumerable<IGrouping<string, BudgetRecordResponse>> GroupRecordsByPeriod(
        List<BudgetRecordResponse> records,
        PeriodEnum period)
    {
        return period switch
        {
            PeriodEnum.Year => records.GroupBy(r => r.RecordDate.ToString("yyyy-MM")), // 年预算按月展示
            PeriodEnum.Month => records.GroupBy(r => r.RecordDate.ToString("yyyy-MM-dd")), // 月预算按日展示
            PeriodEnum.Quarter => records.GroupBy(r => GetWeekOfYear(r.RecordDate)), // 季度预算按周展示
            _ => records.GroupBy(r => r.RecordDate.ToString("yyyy-MM-dd"))
        };
    }

    /// <summary>
    /// 获取日期所在年份的周数
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>周数标识</returns>
    private string GetWeekOfYear(DateTime date)
    {
        var calendar = System.Globalization.CultureInfo.CurrentCulture.Calendar;
        var weekOfYear = calendar.GetWeekOfYear(date,
            System.Globalization.CalendarWeekRule.FirstDay,
            DayOfWeek.Monday);
        return $"{date.Year}-W{weekOfYear:D2}";
    }

    private static ReportInsightPortrait BuildBudgetPortrait(
        List<BudgetProgressReportResponse> progressReports,
        List<BudgetConsumptionTrendReportResponse> trendReports)
    {
        BudgetProgressReportResponse? comprehensive = progressReports.FirstOrDefault(p => p.IsComprehensive);
        decimal totalAmount = comprehensive?.TotalAmount ?? progressReports.Sum(p => p.TotalAmount);
        decimal usedAmount = comprehensive?.UsedAmount ?? progressReports.Sum(p => p.UsedAmount);
        decimal remaining = comprehensive?.Remaining ?? progressReports.Sum(p => p.Remaining);
        var comprehensiveTrends = trendReports
            .Where(p => p.IsComprehensive)
            .OrderBy(p => p.TimePoint)
            .ToList();
        decimal latestTrendAmount = comprehensiveTrends.LastOrDefault()?.ConsumedAmount ?? 0;
        decimal previousTrendAmount = comprehensiveTrends.Count > 1
            ? comprehensiveTrends[^2].ConsumedAmount
            : 0;

        var categories = progressReports
            .Where(p => !p.IsComprehensive)
            .Select(p => new ReportInsightCategoryMetric
            {
                CategoryName = string.IsNullOrWhiteSpace(p.CategoryName) ? "未命名分类" : p.CategoryName,
                BudgetAmount = p.TotalAmount,
                UsedAmount = p.UsedAmount,
                Remaining = p.Remaining,
                UsageRate = p.TotalAmount <= 0 ? 0 : p.UsedAmount / p.TotalAmount,
                Amount = p.UsedAmount,
                Percentage = usedAmount <= 0 ? 0 : p.UsedAmount / usedAmount
            })
            .OrderByDescending(p => p.UsageRate)
            .ToList();

        return new ReportInsightPortrait
        {
            InsightType = "Budget",
            DataPeriod = BuildBudgetDataPeriod(progressReports),
            HasData = progressReports.Count > 0,
            BudgetTotalAmount = totalAmount,
            BudgetUsedAmount = usedAmount,
            BudgetRemaining = remaining,
            BudgetUsageRate = totalAmount <= 0 ? 0 : usedAmount / totalAmount,
            LatestTrendAmount = latestTrendAmount,
            PreviousTrendAmount = previousTrendAmount,
            TrendChangeRate = CalculateChangeRate(latestTrendAmount, previousTrendAmount),
            Categories = categories
        };
    }

    private static string BuildBudgetDataPeriod(List<BudgetProgressReportResponse> progressReports)
    {
        PeriodEnum? period = progressReports.FirstOrDefault(p => !p.IsComprehensive)?.Period
                             ?? progressReports.FirstOrDefault()?.Period;
        return period switch
        {
            PeriodEnum.Year => "当前年度预算周期",
            PeriodEnum.Quarter => "当前季度预算周期",
            PeriodEnum.Month => "当前月度预算周期",
            _ => "当前预算周期"
        };
    }

    private static decimal CalculateChangeRate(decimal current, decimal previous)
    {
        return previous == 0 ? 0 : (current - previous) / previous;
    }
}