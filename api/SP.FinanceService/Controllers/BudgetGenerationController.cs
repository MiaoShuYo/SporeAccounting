
using Microsoft.AspNetCore.Mvc;
using SP.Common.Model;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Controllers;

/// <summary>
/// 预算生成控制器
/// </summary>
[ApiController]
[Route("api/budget-generation")]
public class BudgetGenerationController : ControllerBase
{
    public BudgetGenerationController()
    {
    }

    /// <summary>
    /// AI生成预算，返回预览数据
    /// </summary>
    /// <param name="request">生成请求</param>
    /// <returns>预览数据</returns>
    [HttpPost]
    public ActionResult<List<BudgetGenerationResponse>> GenerateBudget([FromBody] BudgetGenerationRequest request)
    {
        // TODO: 调用AI服务生成预算数据
        return null;
    }

    /// <summary>
    /// 确认/取消生成预算，返回生成结果
    /// </summary>
    /// <param name="id">AI生成预算id</param>
    /// <returns>预算id</returns>
    [HttpPost("{id}/confirm")]
    public ActionResult<long> ConfirmBudget(long id, [FromQuery] bool confirm)
    {
        // TODO: 调用AI服务确认生成预算
        return null;
    }
}
