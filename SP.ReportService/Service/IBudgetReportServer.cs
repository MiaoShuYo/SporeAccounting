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
}