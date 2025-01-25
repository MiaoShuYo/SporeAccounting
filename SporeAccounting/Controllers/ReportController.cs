using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SporeAccounting.BaseModels;
using SporeAccounting.Models.ViewModels;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Controllers
{
    /// <summary>
    /// 报表控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : BaseController
    {
        /// <summary>
        /// 报表服务
        /// </summary>
        private IReportServer _reportServer;

        /// <summary>
        /// 映射
        /// </summary>
        private IMapper _mapper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reportServer"></param>
        /// <param name="mapper"></param>
        public ReportController(IReportServer reportServer, IMapper mapper)
        {
            _reportServer = reportServer;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取报表
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetReport")]
        public ActionResult<ResponseData<List<ReportResponseViewModel>>> GetReport([FromBody] ReportViewModel report)
        {
            try
            {
                string userId = GetUserId();
                var reports = _reportServer.QueryReport(userId, report.Year, report.ReportType);
                List<ReportResponseViewModel> response = _mapper.Map<List<ReportResponseViewModel>>(reports);
                return Ok(new ResponseData<List<ReportResponseViewModel>>(HttpStatusCode.OK, data: response));
            }
            catch (Exception ex)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.BadRequest, errorMessage: ex.Message));
            }
        }
    }
}