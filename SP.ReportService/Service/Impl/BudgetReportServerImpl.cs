using SP.ReportService.Models.Response;
using SP.ReportService.DB;
using SP.ReportService.RefitClient;

namespace SP.ReportService.Service.Impl;

/// <summary>
/// 预算报表服务实现
/// </summary>
public class BudgetReportServerImpl : IBudgetReportServer
{
    private readonly IBudgetServiceApi _budgetServiceApi;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="budgetServiceApi">预算服务API</param>
    public BudgetReportServerImpl(IBudgetServiceApi budgetServiceApi)
    {
        _budgetServiceApi = budgetServiceApi;
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
}