using Microsoft.AspNetCore.Mvc;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Service;

namespace SP.FinanceService.Controllers
{
    /// <summary>
    /// 收支分类接口
    /// </summary>
    [Route("/api/transaction-categories")]
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
        /// 获取指定分类下的所有子分类
        /// </summary>
        /// <param name="parentId">父分类ID</param>
        /// <returns>返回子分类列表</returns>
        [HttpGet("by-parent/{parentId}")]
        public ActionResult<List<TransactionCategoryResponse>> GetCategoriesByParent([FromRoute] long parentId)
        {
            List<TransactionCategoryResponse> categories = _transactionCategoryServer.QueryByParentId(parentId);
            return Ok(categories);
        }

        /// <summary>
        /// 更新收支分类
        /// </summary>
        /// <param name="id">分类ID</param>
        /// <param name="category">收支分类信息</param>
        /// <returns>返回修改结果</returns>
        [HttpPut("{id}")]
        public ActionResult<bool> UpdateCategory([FromRoute] long id, [FromBody] TransactionCategoryEditRequest category)
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
        /// 批量更新父级分类
        /// </summary>
        /// <param name="category">修改父级分类信息</param>
        /// <returns>返回修改结果</returns>
        [HttpPut("update-parent")]
        public ActionResult<bool> UpdateParentCategory([FromBody] TransactionCategoryParentEditRequest category)
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
        [HttpDelete("batch")]
        public ActionResult<bool> DeleteCategories([FromBody] List<long> categoryIds)
        {
            var result = _transactionCategoryServer.Delete(categoryIds);
            return Ok(result);
        }
    }
}