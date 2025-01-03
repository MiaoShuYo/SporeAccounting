using System.Net;
using AutoMapper;
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
                bool isExist = _budgetServer.IsExist(budget.IncomeExpenditureClassificationId, userId);
                if (isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.Found, "用户已存在该类型预算", false));
                }
                Budget budgetDb = _mapper.Map<Budget>(budget);
                budgetDb.UserId = userId;
                budgetDb.CreateDateTime = DateTime.Now;
                _budgetServer.Add(budgetDb);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, "添加成功", true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "添加失败", false));
            }
        }
    }
}