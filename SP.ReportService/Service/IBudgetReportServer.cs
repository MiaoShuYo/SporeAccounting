using SP.ReportService.Models.Response;

namespace SP.ReportService.Service;

/// <summary>
/// 预算报表服务接口
/// </summary>
public interface IBudgetReportServer
{
    /// <summary>
    /// 预算进度
    /// </summary>
    /// <returns>
    /// 预算报表列表包括：
    /// 1. 综合预算进度
    /// 2. 各类别预算进度
    /// </returns>
    Task<List<BudgetProgressReportResponse>> GetBudgetProgress();
    
    /// <summary>
    /// 预算消耗趋势报表
    /// </summary>
    /// <returns>
    /// 预算消耗趋势报表列表包括：
    /// 1. 综合预算消耗趋势
    /// 2. 各类别预算消耗趋势
    /// 年预算消耗趋势报表按月展示，月预算消耗趋势报表按日展示，季度预算消耗趋势报表按周展示
    /// </returns>
    Task<List<BudgetProgressReportResponse>> GetBudgetConsumptionTrend();
}