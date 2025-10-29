using Microsoft.AspNetCore.Mvc;
using Refit;
using SP.ReportService.Models.Response;

namespace SP.ReportService.RefitClient;

/// <summary>
/// 预算记录服务接口
/// </summary>
public interface IBudgetRecordServiceApi
{
    /// <summary>
    /// 获取当前用户正在使用的预算列表
    /// </summary>
    /// <returns>正在使用的预算列表</returns>
    [Get("/api/budget-records/by-budget-ids")]
    Task<ApiResponse<Dictionary<long, List<BudgetRecordResponse>>>> GetBudgetRecordsByBudgetIdsAsync();
}