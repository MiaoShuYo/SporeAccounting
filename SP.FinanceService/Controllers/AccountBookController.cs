using Microsoft.AspNetCore.Mvc;
using SP.Common.Model;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Service;

namespace SP.FinanceService.Controllers;

/// <summary>
/// 账本接口
/// </summary>
[Route("/api/account-books")]
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
    /// 创建账本
    /// </summary>
    /// <param name="request">账本请求</param>
    /// <returns>返回新增账本id</returns>
    [HttpPost]
    public ActionResult<long> CreateAccountBook([FromBody] AccountBookAddRequest request)
    {
        long accountBookId = _accountBookServer.Add(request);
        return Ok(accountBookId);
    }

    /// <summary>
    /// 删除账本
    /// </summary>
    /// <param name="id">账本ID</param>
    /// <returns>返回删除结果</returns>
    [HttpDelete("{id}")]
    public ActionResult<bool> DeleteAccountBook([FromRoute] long id)
    {
        _accountBookServer.Delete(id);
        return Ok();
    }

    /// <summary>
    /// 更新账本
    /// </summary>
    /// <param name="id">账本ID</param>
    /// <param name="request">账本修改请求</param>
    /// <returns>返回修改结果</returns>
    [HttpPut("{id}")]
    public ActionResult<bool> UpdateAccountBook([FromRoute] long id, [FromBody] AccountBookEditeRequest request)
    {
        _accountBookServer.Edit(request);
        return Ok();
    }

    /// <summary>
    /// 分页查询账本列表
    /// </summary>
    /// <param name="page">页码</param>
    /// <param name="size">每页数量</param>
    /// <returns>返回账本列表</returns>
    [HttpGet]
    public ActionResult<PageResponse<AccountBookResponse>> GetAccountBooks(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        var pageRequest = new AccountBookPageRequest
        {
            PageIndex = page,
            PageSize = size
        };
        PageResponse<AccountBookResponse> result = _accountBookServer.QueryPage(pageRequest);
        return Ok(result);
    }
}