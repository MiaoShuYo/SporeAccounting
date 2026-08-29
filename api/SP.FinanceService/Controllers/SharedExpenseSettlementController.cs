using Microsoft.AspNetCore.Mvc;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Service;

namespace SP.FinanceService.Controllers;

/// <summary>
/// 分摊结算接口
/// </summary>
[Route("/api/shared-expense-settlements")]
[ApiController]
public class SharedExpenseSettlementController : ControllerBase
{
    private readonly ISharedExpenseSettlementServer _sharedExpenseSettlementServer;

    public SharedExpenseSettlementController(ISharedExpenseSettlementServer sharedExpenseSettlementServer)
    {
        _sharedExpenseSettlementServer = sharedExpenseSettlementServer;
    }

    /// <summary>
    /// 创建结算记录
    /// </summary>
    /// <param name="request">结算请求</param>
    /// <returns>结算记录Id</returns>
    [HttpPost]
    public ActionResult<long> CreateSettlement([FromBody] SharedExpenseSettlementAddRequest request)
    {
        long id = _sharedExpenseSettlementServer.Add(request);
        return Ok(id);
    }

    /// <summary>
    /// 获取结算详情
    /// </summary>
    /// <param name="id">结算记录Id</param>
    /// <returns>结算详情</returns>
    [HttpGet("{id}")]
    public ActionResult<SharedExpenseSettlementResponse> GetSettlement([FromRoute] long id)
    {
        SharedExpenseSettlementResponse response = _sharedExpenseSettlementServer.QueryById(id);
        return Ok(response);
    }

    /// <summary>
    /// 根据分摊账目Id获取结算记录列表
    /// </summary>
    /// <param name="sharedExpenseId">分摊账目Id</param>
    /// <returns>结算记录列表</returns>
    [HttpGet("by-shared-expense/{sharedExpenseId}")]
    public ActionResult<List<SharedExpenseSettlementResponse>> GetSettlementsBySharedExpense(
        [FromRoute] long sharedExpenseId)
    {
        List<SharedExpenseSettlementResponse> responses =
            _sharedExpenseSettlementServer.QueryBySharedExpenseId(sharedExpenseId);
        return Ok(responses);
    }
}
