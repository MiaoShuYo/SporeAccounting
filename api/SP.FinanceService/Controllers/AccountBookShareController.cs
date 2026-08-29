using Microsoft.AspNetCore.Mvc;
using SP.Common.Model;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Service;

namespace SP.FinanceService.Controllers
{
    [Route("api/account-books/share")]
    [ApiController]
    public class AccountBookShareController : ControllerBase
    {
        /// <summary>
        /// 账本分享服务
        /// </summary>
        private readonly IAccountBookShareServer __accountBookShareServer;

        /// <summary>
        /// 账本分享控制器构造函数
        /// </summary>
        /// <param name="accountBookShareServer"></param>
        public AccountBookShareController(IAccountBookShareServer accountBookShareServer)
        {
            __accountBookShareServer = accountBookShareServer;
        }


        /// <summary>
        /// 分享账本
        /// </summary>
        /// <param name="request">账本分享请求</param>
        /// <returns>返回分享结果</returns>
        [HttpPost]
        public async  Task<ActionResult<bool>> ShareAccountBooks([FromBody] AccountBookShareAddRequest request)
        {
            await __accountBookShareServer.Share(request);
            return Ok();
        }

        /// <summary>
        /// 分页查询我共享的列表
        /// </summary>
        /// <param name="request">账本分享分页请求</param>
        /// <returns>返回分页结果</returns>
        [HttpPost("Page/Self")]
        public ActionResult<PageResponse<AccountBookShareResponse>> PageAccountBookShares(
            [FromBody] AccountBookSharePageRequest request)
        {
            PageResponse<AccountBookShareResponse> result = __accountBookShareServer.Page(request);
            return Ok(result);
        }

        /// <summary>
        /// 获取共享给我的列表
        /// </summary>
        /// <param name="request">账本分享分页请求</param>
        /// <returns>返回分页结果</returns>
        [HttpPost("Page/SharedToMe")]
        public ActionResult<PageResponse<AccountBookShareResponse>> PageAccountBookSharesToMe(
            [FromBody] AccountBookSharePageRequest request)
        {
            PageResponse<AccountBookShareResponse> result = __accountBookShareServer.PageSharesToMe(request);
            return Ok(result);
        }

        /// <summary>
        /// 撤销分享
        /// </summary>
        /// <param name="request">账本分享撤销请求</param>
        /// <returns>返回撤销结果</returns>
        [HttpPut("Revoke")]
        public async Task<ActionResult<bool>> RevokeAccountBookShare([FromBody] AccountBookRevokeSharingRequest request)
        {
            await __accountBookShareServer.Revoke(request);
            return Ok();
        }

        /// <summary>
        /// 修改共享权限
        /// </summary>
        /// <param name="request">账本分享修改请求</param>
        /// <returns>返回修改结果</returns>
        [HttpPut]
        public async Task<ActionResult<bool>> EditAccountBookShare([FromBody] AccountBookShareEditRequest request)
        {
            await __accountBookShareServer.Edit(request);
            return Ok();
        }
    }
}