using Microsoft.AspNetCore.Mvc;
using SP.ResourceService.Service;

namespace SP.ResourceService.Controllers
{
    /// <summary>
    /// AI助手控制器
    /// </summary>
    [Route("api/assistant")]
    [ApiController]
    public class AssistantController : ControllerBase
    {
        /// <summary>
        /// AI助手服务
        /// </summary>
        private readonly IAssistantService _assistantService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="assistantService"></param>
        public AssistantController(IAssistantService assistantService)
        {
            _assistantService = assistantService;
        }

        /// <summary>
        /// 提取文字中的金额和消费类型
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <returns>金额和消费类型</returns>
        [HttpPost("extract-amount-and-category")]
        public async Task<IActionResult> ExtractAmountAndCategory([FromBody] string text)
        {
            var result = await _assistantService.ExtractAmountAndCategoryAsync(text);
            return Ok(result);
        }
    }
}
