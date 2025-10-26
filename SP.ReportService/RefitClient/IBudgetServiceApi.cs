using Refit;
using SP.ReportService.Models.Response;

namespace SP.ReportService.RefitClient;

public interface IBudgetServiceApi
{
    /// <summary>
    /// 获取当前用户正在使用的预算列表
    /// </summary>
    /// <returns>正在使用的预算列表</returns>
    [Get("/api/budget/current-budgets")]
    Task<ApiResponse<List<BudgetResponse>>> GetCurrentBudgetsAsync();
}