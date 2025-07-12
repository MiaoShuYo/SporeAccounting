using Microsoft.AspNetCore.Mvc;
using SP.Common.Model;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Service;

namespace SP.FinanceService.Controllers;

/// <summary>
/// 账本接口
/// </summary>
[Route("/api/accountBook")]
[ApiController]
public class AccountBookController : ControllerBase
{
    private readonly IAccountBookServer _accountBookServer;

    /// <summary>
    /// 账本控制器构造函数
    /// </summary>
    /// <param name="accountBookServer">账本服务</param>
    public AccountBookController(IAccountBookServer accountBookServer)
    {
        _accountBookServer = accountBookServer;
    }

    /// <summary>
    /// 新增账本
    /// </summary>
    /// <param name="request">账本请求</param>
    /// <returns>返回新增账本id</returns>
    [HttpPost("add")]
    public ActionResult<bool> Add([FromBody] AccountBookAddRequest request)
    {
        long accountBookId = _accountBookServer.Add(request);
        return Ok(accountBookId);
    }

    /// <summary>
    /// 删除账本
    /// </summary>
    /// <param name="id">账本ID</param>
    /// <returns>返回删除结果</returns>
    [HttpDelete("delete/{id}")]
    public ActionResult<bool> Delete([FromRoute] long id)
    {
        _accountBookServer.Delete(id);
        return Ok();
    }

    /// <summary>
    /// 修改账本
    /// </summary>
    /// <param name="request">账本修改请求</param>
    /// <returns>返回修改结果</returns>
    [HttpPut("edit")]
    public ActionResult<bool> Edit([FromBody] AccountBookEditeRequest request)
    {
        _accountBookServer.Edit(request);
        return Ok();
    }

    /// <summary>
    /// 分页查询账本列表
    /// </summary>
    /// <param name="page">查询请求</param>
    /// <returns>返回账本列表</returns>
    [HttpGet("page")]
    public ActionResult<PageResponseModel<AccountBookResponse>> Query([FromQuery] AccountBookPageRequest page)
    {
        PageResponseModel<AccountBookResponse> result = _accountBookServer.QueryPage(page);
        return Ok(result);
    }
}