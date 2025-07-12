using Microsoft.AspNetCore.Mvc;
using SP.Common.Model;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Service;

namespace SP.FinanceService.Controllers;

/// <summary>
/// 记账接口
/// </summary>
[Route("/api/accounting")]
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
    /// 新增记账
    /// </summary>
    /// <param name="request">记账请求</param>
    /// <returns>返回记账记录id</returns>
    [HttpPost("add")]
    public ActionResult<long> Add([FromBody] AccountingAddRequest request)
    {
        long accountingId = _accountingServer.Add(request);
        return Ok(accountingId);
    }

    /// <summary>
    /// 删除记账
    /// </summary>
    /// <param name="id">记账ID</param>
    /// <returns>返回删除结果</returns>
    [HttpDelete("delete/{id}")]
    public ActionResult<bool> Delete([FromRoute] long id)
    {
        _accountingServer.Delete(id);
        return Ok(true);
    }

    /// <summary>
    /// 修改记账
    /// </summary>
    /// <param name="request">记账修改请求</param>
    /// <returns>返回修改结果</returns>
    [HttpPut("edit")]
    public ActionResult<bool> Edit([FromBody] AccountingEditRequest request)
    {
        _accountingServer.Edit(request);
        return Ok(true);
    }

    /// <summary>
    /// 查询记账记录详细信息
    /// </summary>
    /// <param name="id">记账ID</param>
    /// <returns>返回记账记录详细信息</returns>
    [HttpGet("query/{id}")]
    public ActionResult<AccountingResponse> Query([FromRoute] long id)
    {
        AccountingResponse accountingRecord = _accountingServer.QueryById(id);
        return Ok(accountingRecord);
    }

    /// <summary>
    /// 分页查询记账记录
    /// </summary>
    /// <param name="accountBookId">账本ID</param>
    /// <param name="page">查询请求</param>
    /// <returns>返回分页查询结果</returns>
    [HttpPost("{accountBookId}/page")]
    public ActionResult<PageResponse<AccountingResponse>> QueryPage([FromRoute] long accountBookId, [FromBody] AccountingPageRequest page)
    {
        page.AccountBookId = accountBookId;
        PageResponse<AccountingResponse> result = _accountingServer.QueryPage(page);
        return Ok(result);
    }
}