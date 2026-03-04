using Microsoft.AspNetCore.Mvc;
using SP.ReportService.Models.Response;
using SP.ReportService.Service;

namespace SP.ReportService.Controllers;

/// <summary>
/// 预算报表控制器
/// </summary>
[Route("api/budget-reports")]
[ApiController]
public class BudgetReportController : ControllerBase
{
    /// <summary>
    /// 预算报表服务
    /// </summary>
    private readonly IBudgetReportServer _budgetReportServer;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="budgetReportServer">预算报表服务</param>
    public BudgetReportController(IBudgetReportServer budgetReportServer)
    {
        _budgetReportServer = budgetReportServer;
    }

    /// <summary>
    /// 预算进度
    /// </summary>
    /// <returns>
    /// 预算报表列表包括：
    /// 1. 综合预算进度
    /// 2. 各类别预算进度
    /// </returns>
    [HttpGet]
    [Route("budget-progress")]
    public async Task<ActionResult<List<BudgetProgressReportResponse>>> GetBudgetProgress()
    {
        List<BudgetProgressReportResponse> budgetProgressReports
            = await _budgetReportServer.GetBudgetProgress();
        return Ok(budgetProgressReports);
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
    [HttpGet]
    [Route("budget-consumption-trend")]
    public async Task<ActionResult<List<BudgetConsumptionTrendReportResponse>>> GetBudgetConsumptionTrend()
    {
        List<BudgetConsumptionTrendReportResponse> budgetProgressReports
            = await _budgetReportServer.GetBudgetConsumptionTrend();
        return Ok(budgetProgressReports);
    }
}