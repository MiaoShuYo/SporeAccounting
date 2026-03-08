using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SP.Common.Model;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Service;

namespace SP.FinanceService.Controllers;

/// <summary>
/// 财务健康评分接口
/// </summary>
[Route("/api/financial-health")]
[ApiController]
public class FinancialHealthScoreController : ControllerBase
{
    private readonly IFinancialHealthScoreService _service;

    /// <summary>
    /// 财务健康评分控制器构造函数
    /// </summary>
    public FinancialHealthScoreController(IFinancialHealthScoreService service)
    {
        _service = service;
    }

    /// <summary>
    /// 手动触发评分计算并保存
    /// </summary>
    /// <param name="accountBookId">账本 ID</param>
    /// <param name="periodStart">统计周期开始日期（yyyy-MM-dd）</param>
    /// <param name="periodEnd">统计周期结束日期（yyyy-MM-dd）</param>
    /// <returns>评分结果</returns>
    [HttpPost("calculate")]
    public async System.Threading.Tasks.Task<ActionResult<FinancialHealthScoreResponse>> Calculate(
        [FromQuery] long accountBookId,
        [FromQuery] DateTime periodStart,
        [FromQuery] DateTime periodEnd)
    {
        var result = await _service.CalculateAndSaveAsync(accountBookId, periodStart, periodEnd);
        return Ok(result);
    }

    /// <summary>
    /// 获取指定账本的最新评分
    /// </summary>
    /// <param name="accountBookId">账本 ID</param>
    /// <returns>最新评分，不存在时返回 null</returns>
    [HttpGet("score")]
    public ActionResult<FinancialHealthScoreResponse?> GetLatestScore([FromQuery] long accountBookId)
    {
        var result = _service.GetLatestScore(accountBookId);
        return Ok(result);
    }

    /// <summary>
    /// 分页获取历史评分记录
    /// </summary>
    /// <param name="accountBookId">账本 ID</param>
    /// <param name="page">页码（默认 1）</param>
    /// <param name="size">每页条数（默认 10）</param>
    /// <returns>历史评分分页列表</returns>
    [HttpGet("history")]
    public ActionResult<PageResponse<FinancialHealthScoreResponse>> GetHistory(
        [FromQuery] long accountBookId,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        var result = _service.GetScoreHistory(accountBookId, page, size);
        return Ok(result);
    }

    /// <summary>
    /// 获取当月财务改善建议
    /// </summary>
    /// <param name="accountBookId">账本 ID</param>
    /// <returns>改善建议列表</returns>
    [HttpGet("suggestions")]
    public async System.Threading.Tasks.Task<ActionResult<List<FinancialSuggestionResponse>>> GetSuggestions(
        [FromQuery] long accountBookId)
    {
        var result = await _service.GetSuggestionsAsync(accountBookId);
        return Ok(result);
    }
}
