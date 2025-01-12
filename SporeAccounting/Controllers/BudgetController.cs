using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SporeAccounting.BaseModels;
using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Controllers
{
    /// <summary>
    /// 预算控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Consumer,Administrator")]
    public class BudgetController : BaseController
    {
        /// <summary>
        /// 预算服务
        /// </summary>
        private IBudgetServer _budgetServer;

        private IMapper _mapper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="budgetServer"></param>
        /// <param name="mapper"></param>
        public BudgetController(IBudgetServer budgetServer, IMapper mapper)
        {
            _budgetServer = budgetServer;
            _mapper = mapper;
        }

        /// <summary>
        /// 添加预算
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Add")]
        public ActionResult<ResponseData<bool>> Add([FromBody] BudgetAddViewModel budget)
        {
            try
            {
                string userId = GetUserId();
                // 用户是否存在该类型预算
                bool isExist = _budgetServer.IsExistByClassificationId(budget.ClassificationId, userId);
                if (isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.Found, "用户已存在该类型预算", false));
                }
                Budget budgetDb = _mapper.Map<Budget>(budget);
                budgetDb.Remaining = budgetDb.Amount;
                budgetDb.UserId = userId;
                budgetDb.CreateUserId = userId;
                budgetDb.CreateDateTime = DateTime.Now;
                _budgetServer.Add(budgetDb);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, "添加成功", true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "添加失败", false));
            }
        }
        
        /// <summary>
        /// 删除预算
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete/{id}")]
        public ActionResult<ResponseData<bool>> Delete(string id)
        {
            try
            {
                // 预算是否存在
                bool isExist = _budgetServer.IsExist(id);
                if (!isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, "预算不存在", false));
                }
                // 预算是否是当前用户的
                string userId = GetUserId();
                bool isYou = _budgetServer.IsYou(id, userId);
                if (!isYou)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.Forbidden, "不是你的预算", false));
                }
                _budgetServer.Delete(id);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, "删除成功", true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "删除失败", false));
            }
        }
        
        /// <summary>
        /// 修改预算
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Update")]
        public ActionResult<ResponseData<bool>> Update([FromBody] BudgetUpdateViewModel budget)
        {
            try
            {
                // 预算是否存在
                bool isExist = _budgetServer.IsExist(budget.Id);
                if (!isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, "预���不存在", false));
                }
                // 预算是否是当前用户的
                string userId = GetUserId();
                bool isYou = _budgetServer.IsYou(budget.Id, userId);
                if (!isYou)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.Forbidden, "不是你的预算", false));
                }
                Budget budgetDb = _mapper.Map<Budget>(budget);
                _budgetServer.Update(budgetDb);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, "修改成功", true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "修改失败", false));
            }
        }
        
        /// <summary>
        /// 查询预算
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Query")]
        public ActionResult<ResponseData<List<BudgetViewModel>>> Query()
        {
            try
            {
                string userId = GetUserId();
                List<Budget> budgets = _budgetServer.Query(userId);
                List<BudgetViewModel> budgetViewModels = _mapper.Map<List<BudgetViewModel>>(budgets);
                return Ok(new ResponseData<List<BudgetViewModel>>(HttpStatusCode.OK, data: budgetViewModels));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<List<BudgetViewModel>>(HttpStatusCode.InternalServerError, "查询失败"));
            }
        }
    }
}