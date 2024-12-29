using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using SporeAccounting.BaseModels.ViewModel.Response;
using Microsoft.AspNetCore.Mvc;
using SporeAccounting.BaseModels;
using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Controllers
{
    /// <summary>
    /// 账本控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Consumer,Administrator")]
    public class AccountBookController : BaseController
    {
        /// <summary>
        /// 账本服务
        /// </summary>
        private readonly IAccountBookServer _accountBookServer;

        /// <summary>
        /// 映射
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="accountBookServer"></param>
        /// <param name="mapper"></param>
        public AccountBookController(IAccountBookServer accountBookServer, IMapper mapper)
        {
            _accountBookServer = accountBookServer;
            _mapper = mapper;
        }

        /// <summary>
        /// 新增账本
        /// </summary>
        /// <param name="accountBookAdd"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Add")]
        public ActionResult<ResponseData<bool>> Add([FromBody] AccountBookAddViewmModel accountBookAdd)
        {
            try
            {
                string userId = GetUserId();
                //是否存在同名的账本
                bool isExist = _accountBookServer.IsExist(accountBookAdd.Name, userId);
                if (isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.BadRequest,
                        errorMessage: $"账本{accountBookAdd.Name}已存在"));
                }

                //保存账本
                AccountBook accountBook = _mapper.Map<AccountBook>(accountBookAdd);
                accountBook.UserId = userId;
                accountBook.CreateDateTime = DateTime.Now;
                accountBook.CreateUserId = userId;
                _accountBookServer.Add(accountBook);

                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, errorMessage: "服务端异常"));
            }
        }

        /// <summary>
        /// 删除账本
        /// </summary>
        /// <param name="accountBookId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete/{accountBookId}")]
        public ActionResult<ResponseData<bool>> Delete([FromRoute] string accountBookId)
        {
            try
            {
                //是否存在账本
                bool isExist = _accountBookServer.IsExistById(accountBookId);
                if (!isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.BadRequest, errorMessage: "账本不存在"));
                }

                //是否存在收支记录
                bool isExistIncomeExpenditureRecord = _accountBookServer.IsExistIncomeExpenditureRecord(accountBookId);
                if (isExistIncomeExpenditureRecord)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.BadRequest, errorMessage: "账本存在收支记录，不能删除"));
                }

                _accountBookServer.Delete(accountBookId);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, errorMessage: "服务端异常"));
            }
        }

        /// <summary>
        /// 修改账本
        /// </summary>
        /// <param name="accountBookUpdate"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Update")]
        public ActionResult<ResponseData<bool>> Update([FromBody] AccountBookUpdateViewModel accountBookUpdate)
        {
            try
            {
                string userId = GetUserId();
                //是否存在账本
                bool isExist = _accountBookServer.IsExistById(accountBookUpdate.AccountBookId);
                if (!isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.BadRequest, errorMessage: "账本不存在"));
                }

                //是否存在同名的账本
                bool isExistName = _accountBookServer.IsExist(accountBookUpdate.Name, accountBookUpdate.AccountBookId,
                    userId);
                if (isExistName)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.BadRequest,
                        errorMessage: $"账本{accountBookUpdate.Name}已存在"));
                }

                AccountBook accountBook = _mapper.Map<AccountBook>(accountBookUpdate);
                accountBook.UpdateDateTime = DateTime.Now;
                accountBook.UpdateUserId = userId;
                _accountBookServer.Update(accountBook);

                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, errorMessage: "服务端异常"));
            }
        }

        /// <summary>
        /// 查询账本
        /// </summary>
        /// <param name="accountBookId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Query/{accountBookId}")]
        public ActionResult<ResponseData<AccountBookInfoViewModel>> Query([FromRoute] string accountBookId)
        {
            try
            {
                AccountBook accountBook = _accountBookServer.Query(accountBookId);
                AccountBookInfoViewModel accountBookInfo = _mapper.Map<AccountBookInfoViewModel>(accountBook);
                return Ok(new ResponseData<AccountBookInfoViewModel>(HttpStatusCode.OK, data: accountBookInfo));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<AccountBookInfoViewModel>(HttpStatusCode.InternalServerError,
                    errorMessage: "服务端异常"));
            }
        }

        /// <summary>
        /// 分页查询账本
        /// </summary>
        /// <param name="accountBookPage"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Query")]
        public ActionResult<ResponseData<PageResponseViewModel<AccountBookInfoViewModel>>> Query(
            [FromBody] AccountBookPageViewModel accountBookPage)
        {
            try
            {
                string userId = GetUserId();
                var (rowCount, pageCount, accountBooks) =
                    _accountBookServer.Query(accountBookPage.PageNumber, accountBookPage.PageSize, userId);
                PageResponseViewModel<AccountBookInfoViewModel> pagination =
                    new PageResponseViewModel<AccountBookInfoViewModel>
                    {
                        RowCount = rowCount,
                        Data = _mapper.Map<List<AccountBookInfoViewModel>>(accountBooks),
                        PageCount = pageCount
                    };
                return Ok(new ResponseData<PageResponseViewModel<AccountBookInfoViewModel>>(HttpStatusCode.OK,
                    data: pagination));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<PageResponseViewModel<AccountBookInfoViewModel>>(
                    HttpStatusCode.InternalServerError,
                    errorMessage: "服务端异常"));
            }
        }
    }
}