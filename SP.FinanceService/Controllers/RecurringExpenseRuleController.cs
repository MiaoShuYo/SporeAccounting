using Microsoft.AspNetCore.Mvc;
using SP.Common.Model;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Service;

namespace SP.FinanceService.Controllers;

/// <summary>
/// 规定开销规则接口
/// </summary>
[Route("api/recurring-expense-rule")]
[ApiController]
public class RecurringExpenseRuleController : ControllerBase
{

    /// <summary>
    /// 规定开销规则服务
    /// </summary>
    private readonly IRecurringExpenseRuleServer _recurringExpenseRuleServer;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="recurringExpenseRuleServer"></param>
    public RecurringExpenseRuleController(IRecurringExpenseRuleServer recurringExpenseRuleServer)
    {
        _recurringExpenseRuleServer = recurringExpenseRuleServer;
    }

    /// <summary>
    /// 新增开销规则
    /// </summary>
    /// <param name="recurringExpenseRuleAdd">新增请求</param>
    /// <returns>是否成功</returns>
    [HttpPost]
    public  async Task<ActionResult<long>> AddRecurringExpenseRule([FromBody] RecurringExpenseRuleAddRequest recurringExpenseRuleAdd)
    {
        long id = await _recurringExpenseRuleServer.AddRecurringExpenseRule(recurringExpenseRuleAdd);
        return Ok(id);
    }

    /// <summary>
    /// 编辑开销规则
    /// </summary>
    /// <param name="recurringExpenseRuleEdit">修改请求</param>
    /// <returns></returns>
    [HttpPut]
    public async  Task<ActionResult<long>> EditRecurringExpenseRule([FromBody] RecurringExpenseRuleEditRequest recurringExpenseRuleEdit)
    {
        long id = await _recurringExpenseRuleServer.EditRecurringExpenseRule(recurringExpenseRuleEdit);
        return Ok(id);
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids">删除的数据id</param>
    [HttpPost]
    public async Task<ActionResult<bool>> DeleteRecurringExpenseRule([FromBody] List<long> ids)
    {
        await _recurringExpenseRuleServer.DeleteRecurringExpenseRule(ids);
        return Ok(true);
    }

    /// <summary>
    /// 获取规定开销规则
    /// </summary>
    /// <param name="id">规定开销规则id</param>
    /// <returns>规定开销规则</returns>
    [HttpGet]
    public ActionResult<RecurringExpenseRuleResponse> GetRecurringExpenseRule([FromRoute] long id)
    {
        RecurringExpenseRuleResponse response = _recurringExpenseRuleServer.GetRecurringExpenseRule(id);
        return Ok(response);
    }

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="page">分页信息</param>
    /// <returns>规定开销规则列表</returns>
    [HttpPost("/page")]
    public ActionResult<PageResponse<RecurringExpenseRuleResponse>> GetRecurringExpenseRulePage([FromBody] RecurringExpenseRulePgRequest page)
    {
        PageResponse<RecurringExpenseRuleResponse> response = _recurringExpenseRuleServer.GetRecurringExpenseRulePage(page);
        return Ok(response);
    }
}