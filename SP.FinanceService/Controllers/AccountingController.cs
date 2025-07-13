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
    /// <param name="accountBookId">账本ID</param>
    /// <param name="request">记账请求</param>
    /// <returns>返回记账记录id</returns>
    [HttpPost("{accountBookId}/add")]
    public ActionResult<long> Add([FromRoute] long accountBookId, [FromBody] AccountingAddRequest request)
    {
        long accountingId = _accountingServer.Add(accountBookId,request);
        return Ok(accountingId);
    }

    /// <summary>
    /// 删除记账
    /// </summary>
    /// <param name="id">记账ID</param>
    /// <returns>返回删除结果</returns>
    [HttpDelete("{accountBookId}/delete/{id}")]
    public ActionResult<bool> Delete([FromRoute] long accountBookId,[FromRoute] long id)
    {
        _accountingServer.Delete(accountBookId,id);
        return Ok(true);
    }

    /// <summary>
    /// 修改记账
    /// </summary>
    /// <param name="request">记账修改请求</param>
    /// <returns>返回修改结果</returns>
    [HttpPut("/{accountBookId}/edit")]
    public ActionResult<bool> Edit([FromRoute] long accountBookId,[FromBody] AccountingEditRequest request)
    {
        _accountingServer.Edit(accountBookId,request);
        return Ok(true);
    }

    /// <summary>
    /// 查询记账记录详细信息
    /// </summary>
    /// <param name="id">记账ID</param>
    /// <returns>返回记账记录详细信息</returns>
    [HttpGet("{accountBookId}/query/{id}")]
    public ActionResult<AccountingResponse> Query([FromRoute] long accountBookId,[FromRoute] long id)
    {
        AccountingResponse accountingRecord = _accountingServer.QueryById(accountBookId,id);
        return Ok(accountingRecord);
    }

    /// <summary>
    /// 分页查询记账记录
    /// </summary>
    /// <param name="accountBookId">账本ID</param>
    /// <param name="page">查询请求</param>
    /// <returns>返回分页查询结果</returns>
    [HttpPost("{accountBookId}/page")]
    public ActionResult<PageResponse<AccountingResponse>> QueryPage([FromRoute] long accountBookId,
        [FromBody] AccountingPageRequest page)
    {
        PageResponse<AccountingResponse> result = _accountingServer.QueryPage(accountBookId,page);
        return Ok(result);
    }
}