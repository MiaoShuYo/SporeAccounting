using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Service;

namespace SP.FinanceService.Controllers
{
    /// <summary>
    /// 收支分类接口
    /// </summary>
    [Route("/api/transactionCategory")]
    [ApiController]
    public class TransactionCategoryController : ControllerBase
    {
        /// <summary>
        /// 收支分类服务
        /// </summary>
        private readonly ITransactionCategoryServer _transactionCategoryServer;

        /// <summary>
        /// 收支分类控制器构造函数
        /// </summary>
        /// <param name="transactionCategoryServer"></param>
        public TransactionCategoryController(ITransactionCategoryServer transactionCategoryServer)
        {
            _transactionCategoryServer = transactionCategoryServer;
        }

        /// <summary>
        /// 查询指定分类下的所有子分类
        /// </summary>
        /// <param name="parentId">父分类ID</param>
        /// <returns>返回子分类列表</returns>
        [HttpGet("queryByParentId/{parentId}")]
        public ActionResult<List<TransactionCategoryResponse>> QueryByParentId([FromRoute] long parentId)
        {
            List<TransactionCategoryResponse> categories = _transactionCategoryServer.QueryByParentId(parentId);
            return Ok(categories);
        }
    }
}