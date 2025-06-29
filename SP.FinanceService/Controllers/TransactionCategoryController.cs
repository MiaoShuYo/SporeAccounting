using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP.FinanceService.Models.Request;
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
        [HttpGet("{parentId}/query")]
        public ActionResult<List<TransactionCategoryResponse>> QueryByParentId([FromRoute] long parentId)
        {
            List<TransactionCategoryResponse> categories = _transactionCategoryServer.QueryByParentId(parentId);
            return Ok(categories);
        }

        /// <summary>
        /// 修改收支分类
        /// </summary>
        /// <param name="category">收支分类信息</param>
        /// <returns>返回修改结果</returns>
        [HttpPut("edit")]
        public ActionResult<bool> Edit([FromBody] TransactionCategoryEditRequest category)
        {
            if (category == null || category.Id <= 0)
            {
                return BadRequest("Invalid category data.");
            }

            bool result = _transactionCategoryServer.Edit(category);
            if (result)
            {
                return Ok(true);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update category.");
            }
        }

        /// <summary>
        /// 批量修改父级分类
        /// </summary>
        /// <param name="category">修改父级分类信息</param>
        /// <returns>返回修改结果</returns>
        [HttpPut("editParent")]
        public ActionResult<bool> EditParent([FromBody] TransactionCategoryParentEditRequest category)
        {
            bool result = _transactionCategoryServer.EditParent(category);
            if (result)
            {
                return Ok(true);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update parent category.");
            }
        }

        /// <summary>
        /// 批量删除收支分类
        /// </summary>
        /// <param name="categoryIds">要删除的分类ID列表</param>
        /// <returns>返回删除结果</returns>
        [HttpPost("delete")]
        public ActionResult<bool> Delete([FromBody] List<long> categoryIds)
        {
            var result = _transactionCategoryServer.Delete(categoryIds);
            return Ok(result);
        }
    }
}