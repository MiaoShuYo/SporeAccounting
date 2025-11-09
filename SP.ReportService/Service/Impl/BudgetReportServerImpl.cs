using SP.ReportService.Models.Response;
using SP.ReportService.Models.Enumeration;
using SP.ReportService.DB;
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
    /// 构造函数
    /// </summary>
    /// <param name="budgetServiceApi">预算服务API</param>
    /// <param name="budgetRecordServiceApi">预算记录服务API</param>
    public BudgetReportServerImpl(IBudgetServiceApi budgetServiceApi, IBudgetRecordServiceApi budgetRecordServiceApi)
    {
        _budgetServiceApi = budgetServiceApi;
        _budgetRecordServiceApi = budgetRecordServiceApi;
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
    public async Task<List<BudgetProgressReportResponse>> GetBudgetConsumptionTrend()
    {
        // 1. 获取预算记录
        var result = await _budgetRecordServiceApi.GetBudgetRecordsByBudgetIdsAsync();
        if (result.IsSuccessStatusCode && result.Content != null)
        {
            var budgetRecords = result.Content;
            List<BudgetProgressReportResponse> trendReports = new List<BudgetProgressReportResponse>();

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

        return new List<BudgetProgressReportResponse>();
    }

    /// <summary>
    /// 计算综合预算消耗趋势
    /// </summary>
    /// <param name="budgetRecords">预算记录</param>
    /// <param name="budgets">预算信息</param>
    /// <returns>综合预算消耗趋势报表</returns>
    private List<BudgetProgressReportResponse> CalculateComprehensiveTrend(
        Dictionary<long, List<BudgetRecordResponse>> budgetRecords,
        List<BudgetResponse> budgets)
    {
        var trendReports = new List<BudgetProgressReportResponse>();

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
                var totalBudgetAmount = periodBudgets.Sum(b => b.Amount);
                var totalRemaining = periodBudgets.Sum(b => b.Remaining);

                trendReports.Add(new BudgetProgressReportResponse
                {
                    Period = period,
                    IsComprehensive = true,
                    CategoryName = "综合预算",
                    TotalAmount = totalBudgetAmount,
                    UsedAmount = totalUsedAmount,
                    Remaining = totalRemaining,
                    ReportDate = group.Key
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
    private List<BudgetProgressReportResponse> CalculateCategoryTrends(
        Dictionary<long, List<BudgetRecordResponse>> budgetRecords,
        List<BudgetResponse> budgets)
    {
        var trendReports = new List<BudgetProgressReportResponse>();

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
                var totalBudgetAmount = periodBudgets.Sum(b => b.Amount);
                var totalRemaining = periodBudgets.Sum(b => b.Remaining);

                trendReports.Add(new BudgetProgressReportResponse
                {
                    Period = period,
                    IsComprehensive = false,
                    CategoryName = categoryName,
                    TotalAmount = totalBudgetAmount,
                    UsedAmount = totalUsedAmount,
                    Remaining = totalRemaining,
                    ReportDate = recordGroup.Key
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
}