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
    public ActionResult<List<BudgetProgressReportResponse>> GetBudgetProgress()
    {
        List<BudgetProgressReportResponse> budgetProgressReports
            = _budgetReportServer.GetBudgetProgress().Result;
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
    public ActionResult<List<BudgetConsumptionTrendReportResponse>> GetBudgetConsumptionTrend()
    {
        //TODO: 预算消耗趋势报表数据
        return null;
    }

    /// <summary>
    /// 预算与实际对比报表
    /// </summary>
    /// <returns>
    /// 预算与实际对比报表列表包括：
    /// 1. 综合预算与实际对比
    /// 2. 各类别预算与实际对比
    /// </returns>
    [HttpGet]
    [Route("budget-actual-comparison")]
    public ActionResult<List<BudgetActualComparisonReportResponse>> GetBudgetActualComparison()
    {
        //TODO: 预算与实际对比报表数据
        return null;
    }

    /// <summary>
    /// 最近N个历史预算对比报告
    /// </summary>
    /// <param name="n">最近N个预算，默认值为3</param>
    /// <returns>
    /// 历史预算对比报表列表包括：
    /// 1. 综合历史预算对比
    /// 2. 各类别历史预算对比
    /// </returns>
    [HttpGet]
    [Route("historical-budget-comparison")]
    public ActionResult<List<HistoricalBudgetComparisonReportResponse>> GetHistoricalBudgetComparison(int n = 3)
    {
        //TODO: 历史预算对比报表数据
        return null;
    }
}