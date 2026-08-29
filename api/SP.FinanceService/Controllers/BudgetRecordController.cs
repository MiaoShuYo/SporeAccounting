using Microsoft.AspNetCore.Mvc;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Service;

namespace SP.FinanceService.Controllers
{
    /// <summary>
    /// 预算记录控制器
    /// </summary>
    [Route("api/budget-records")]
    [ApiController]
    public class BudgetRecordController : ControllerBase
    {
        /// <summary>
        /// 预算记录服务
        /// </summary>
        private readonly IBudgetRecordServer _budgetRecordServer;

        /// <summary>
        /// 预算记录控制器构造函数
        /// </summary>
        /// <param name="budgetRecordServer">预算记录服务</param>
        public BudgetRecordController(IBudgetRecordServer budgetRecordServer)
        {
            _budgetRecordServer = budgetRecordServer;
        }

        /// <summary>
        /// 根据预算Id集合获取预算记录
        /// </summary>
        /// <returns>预算记录集合</returns>
        [HttpGet("by-budget-ids")]
        public ActionResult<Dictionary<long, List<BudgetRecordResponse>>> GetBudgetRecordsByBudgetIds()
        {
            var result = _budgetRecordServer.GetBudgetRecordsByBudgetIds();
            return Ok(result);
        }
    }
}