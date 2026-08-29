using Microsoft.AspNetCore.Mvc;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Service;

namespace SP.FinanceService.Controllers;

/// <summary>
/// 分摊提醒接口
/// </summary>
[Route("/api/shared-expense-reminders")]
[ApiController]
public class SharedExpenseReminderController : ControllerBase
{
    private readonly ISharedExpenseReminderServer _sharedExpenseReminderServer;

    public SharedExpenseReminderController(ISharedExpenseReminderServer sharedExpenseReminderServer)
    {
        _sharedExpenseReminderServer = sharedExpenseReminderServer;
    }

    /// <summary>
    /// 创建提醒
    /// </summary>
    /// <param name="request">提醒请求</param>
    /// <returns>提醒记录Id</returns>
    [HttpPost]
    public ActionResult<long> CreateReminder([FromBody] SharedExpenseReminderAddRequest request)
    {
        long id = _sharedExpenseReminderServer.Add(request);
        return Ok(id);
    }

    /// <summary>
    /// 根据分摊账目Id获取提醒记录
    /// </summary>
    /// <param name="sharedExpenseId">分摊账目Id</param>
    /// <returns>提醒记录列表</returns>
    [HttpGet("by-shared-expense/{sharedExpenseId}")]
    public ActionResult<List<SharedExpenseReminderResponse>> GetReminders([FromRoute] long sharedExpenseId)
    {
        List<SharedExpenseReminderResponse> responses =
            _sharedExpenseReminderServer.QueryBySharedExpenseId(sharedExpenseId);
        return Ok(responses);
    }

    /// <summary>
    /// 更新提醒状态
    /// </summary>
    /// <param name="id">提醒记录Id</param>
    /// <param name="request">状态更新请求</param>
    /// <returns>更新结果</returns>
    [HttpPut("{id}/status")]
    public ActionResult<bool> UpdateStatus([FromRoute] long id, [FromBody] SharedExpenseReminderStatusUpdateRequest request)
    {
        if (request == null || request.Id <= 0)
        {
            return BadRequest("Invalid reminder data.");
        }

        if (id != request.Id)
        {
            return BadRequest("Route id does not match request.Id.");
        }

        _sharedExpenseReminderServer.UpdateStatus(request.Id, request.Status, null);
        return Ok(true);
    }
}
