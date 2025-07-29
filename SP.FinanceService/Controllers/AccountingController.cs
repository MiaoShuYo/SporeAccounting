using Microsoft.AspNetCore.Mvc;
using SP.Common.Model;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Service;

namespace SP.FinanceService.Controllers;

/// <summary>
/// 记账接口
/// </summary>
[Route("/api/accountings")]
[ApiController]
public class AccountingController : ControllerBase
{
    /// <summary>
    /// 记账服务
    /// </summary>
    private readonly IAccountingServer _accountingServer;

    /// <summary>
    /// 记账控制器构造函数
    /// </summary>
    public AccountingController(IAccountingServer accountingServer)
    {
        _accountingServer = accountingServer;
    }

    /// <summary>
    /// 创建记账记录
    /// </summary>
    /// <param name="request">记账请求</param>
    /// <returns>返回记账记录id</returns>
    [HttpPost]
    public ActionResult<long> CreateAccounting([FromBody] AccountingAddRequest request)
    {
        long accountingId = _accountingServer.Add(request.AccountBookId, request);
        return Ok(accountingId);
    }

    /// <summary>
    /// 删除记账记录
    /// </summary>
    /// <param name="id">记账ID</param>
    /// <returns>返回删除结果</returns>
    [HttpDelete("{id}")]
    public ActionResult<bool> DeleteAccounting([FromRoute] long id)
    {
        _accountingServer.Delete(id, id);
        return Ok(true);
    }

    /// <summary>
    /// 更新记账记录
    /// </summary>
    /// <param name="id">记账ID</param>
    /// <param name="request">记账修改请求</param>
    /// <returns>返回修改结果</returns>
    [HttpPut("{id}")]
    public ActionResult<bool> UpdateAccounting([FromRoute] long id, [FromBody] AccountingEditRequest request)
    {
        _accountingServer.Edit(request.AccountBookId, request);
        return Ok(true);
    }

    /// <summary>
    /// 获取记账记录详细信息
    /// </summary>
    /// <param name="id">记账ID</param>
    /// <returns>返回记账记录详细信息</returns>
    [HttpGet("{id}")]
    public ActionResult<AccountingResponse> GetAccounting([FromRoute] long id)
    {
        AccountingResponse accountingRecord = _accountingServer.QueryById(id, id);
        return Ok(accountingRecord);
    }

    /// <summary>
    /// 分页获取记账记录列表
    /// </summary>
    /// <param name="accountBookId">账本ID</param>
    /// <param name="page">页码</param>
    /// <param name="size">每页数量</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <returns>返回分页查询结果</returns>
    [HttpGet]
    public ActionResult<PageResponse<AccountingResponse>> GetAccountings(
        [FromQuery] long accountBookId,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] DateTime? startTime = null,
        [FromQuery] DateTime? endTime = null)
    {
        var request = new AccountingPageRequest
        {
            AccountBookId = accountBookId,
            PageIndex = page,
            PageSize = size,
            StartTime = startTime,
            EndTime = endTime
        };
        PageResponse<AccountingResponse> result = _accountingServer.QueryPage(accountBookId, request);
        return Ok(result);
    }
}