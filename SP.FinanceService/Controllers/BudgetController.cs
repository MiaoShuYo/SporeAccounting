using Microsoft.AspNetCore.Mvc;
using SP.Common.Model;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Service;

namespace SP.FinanceService.Controllers
{
    /// <summary>
    /// 预算控制器
    /// </summary>
    [Route("api/budget")]
    [ApiController]
    public class BudgetController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        private readonly IBudgetServer _budgetServer;

        /// <summary>
        /// 预算控制器构造函数
        /// </summary>
        /// <param name="budgetServer"></param>
        public BudgetController(IBudgetServer budgetServer)
        {
            _budgetServer = budgetServer;
        }

        /// <summary>
        /// 新增预算
        /// </summary>
        /// <param name="budget">预算</param>
        /// <returns>预算id</returns>
        [HttpPost("add")]
        public ActionResult<long> AddBudget([FromBody] BudgetAddRequest budget)
        {
            long id = _budgetServer.Add(budget);
            return Ok(id);
        }

        /// <summary>
        /// 删除预算
        /// </summary>
        /// <param name="id">预算ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("delete/{id}")]
        public ActionResult<bool> DeleteBudget([FromRoute] long id)
        {
            _budgetServer.Delete(id);
            return Ok(true);
        }

        /// <summary>
        /// 修改预算
        /// </summary>
        /// <param name="budget">预算修改请求</param>
        /// <returns>修改结果</returns>
        [HttpPut("edit")]
        public ActionResult<bool> EditBudget([FromBody] BudgetEditRequest budget)
        {
            _budgetServer.Edit(budget);
            return Ok(true);
        }

        /// <summary>·
        /// 获取预算列表
        /// </summary>
        /// <param name="request">分页查询请求</param>
        /// <returns>预算分页列表</returns>
        [HttpPost("page")]
        public ActionResult<PageResponse<BudgetResponse>> QueryPage([FromBody] BudgetPageRequest request)
        {
            PageResponse<BudgetResponse> budgets = _budgetServer.QueryPage(request);
            return Ok(budgets);
        }

        /// <summary>
        /// 获取预算信息
        /// </summary>
        /// <param name="id">预算ID</param>
        /// <returns>预算信息</returns>
        [HttpGet("query/{id}")]
        public ActionResult<BudgetResponse> QueryById([FromRoute] long id)
        {
            BudgetResponse budget = _budgetServer.QueryById(id);
            return Ok(budget);
        }
    }
}