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
    [Route("api/budgets")]
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
        /// 创建预算
        /// </summary>
        /// <param name="budget">预算</param>
        /// <returns>预算id</returns>
        [HttpPost]
        public ActionResult<long> CreateBudget([FromBody] BudgetAddRequest budget)
        {
            long id = _budgetServer.Add(budget);
            return Ok(id);
        }

        /// <summary>
        /// 删除预算
        /// </summary>
        /// <param name="id">预算ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("{id}")]
        public ActionResult<bool> DeleteBudget([FromRoute] long id)
        {
            _budgetServer.Delete(id);
            return Ok(true);
        }

        /// <summary>
        /// 更新预算
        /// </summary>
        /// <param name="id">预算ID</param>
        /// <param name="budget">预算修改请求</param>
        /// <returns>修改结果</returns>
        [HttpPut("{id}")]
        public ActionResult<bool> UpdateBudget([FromRoute] long id, [FromBody] BudgetEditRequest budget)
        {
            _budgetServer.Edit(budget);
            return Ok(true);
        }

        /// <summary>
        /// 分页获取预算列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">每页数量</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>预算分页列表</returns>
        [HttpGet]
        public ActionResult<PageResponse<BudgetResponse>> GetBudgets(
            [FromQuery] int page = 1, 
            [FromQuery] int size = 10,
            [FromQuery] int year = 0,
            [FromQuery] int month = 0)
        {
            var request = new BudgetPageRequest
            {
                PageIndex = page,
                PageSize = size,
                Year = year,
                Month = month
            };
            PageResponse<BudgetResponse> budgets = _budgetServer.QueryPage(request);
            return Ok(budgets);
        }

        /// <summary>
        /// 获取预算信息
        /// </summary>
        /// <param name="id">预算ID</param>
        /// <returns>预算信息</returns>
        [HttpGet("{id}")]
        public ActionResult<BudgetResponse> GetBudget([FromRoute] long id)
        {
            BudgetResponse budget = _budgetServer.QueryById(id);
            return Ok(budget);
        }
    }
}