using Microsoft.AspNetCore.Mvc;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Service;

namespace SP.FinanceService.Controllers;

/// <summary>
/// 分摊账目接口
/// </summary>
[Route("/api/shared-expenses")]
[ApiController]
public class SharedExpenseController : ControllerBase
{
    private readonly ISharedExpenseServer _sharedExpenseServer;

    public SharedExpenseController(ISharedExpenseServer sharedExpenseServer)
    {
        _sharedExpenseServer = sharedExpenseServer;
    }

    /// <summary>
    /// 创建分摊账目
    /// </summary>
    /// <param name="request">分摊账目请求</param>
    /// <returns>分摊账目Id</returns>
    [HttpPost]
    public ActionResult<long> CreateSharedExpense([FromBody] SharedExpenseAddRequest request)
    {
        long id = _sharedExpenseServer.Add(request);
        return Ok(id);
    }

    /// <summary>
    /// 获取分摊账目详情
    /// </summary>
    /// <param name="id">分摊账目Id</param>
    /// <returns>分摊账目详情</returns>
    [HttpGet("{id}")]
    public ActionResult<SharedExpenseResponse> GetSharedExpense([FromRoute] long id)
    {
        SharedExpenseResponse response = _sharedExpenseServer.QueryById(id);
        return Ok(response);
    }

    /// <summary>
    /// 更新分摊账目
    /// </summary>
    /// <param name="id">分摊账目Id</param>
    /// <param name="request">分摊账目编辑请求</param>
    /// <returns>修改结果</returns>
    [HttpPut("{id}")]
    public ActionResult<bool> UpdateSharedExpense([FromRoute] long id, [FromBody] SharedExpenseEditRequest request)
    {
        if (request == null || request.Id <= 0)
        {
            return BadRequest("Invalid shared expense data.");
        }

        if (id != request.Id)
        {
            return BadRequest("Route id does not match request.Id.");
        }

        _sharedExpenseServer.Edit(request);
        return Ok(true);
    }

    /// <summary>
    /// 删除分摊账目
    /// </summary>
    /// <param name="id">分摊账目Id</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    public ActionResult<bool> DeleteSharedExpense([FromRoute] long id)
    {
        _sharedExpenseServer.Delete(id);
        return Ok(true);
    }

    /// <summary>
    /// 根据账本Id获取分摊账目列表
    /// </summary>
    /// <param name="accountBookId">账本Id</param>
    /// <returns>分摊账目列表</returns>
    [HttpGet]
    public ActionResult<List<SharedExpenseResponse>> GetSharedExpenses([FromQuery] long accountBookId)
    {
        List<SharedExpenseResponse> responses = _sharedExpenseServer.QueryByAccountBookId(accountBookId);
        return Ok(responses);
    }
}
