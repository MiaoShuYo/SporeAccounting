using Microsoft.AspNetCore.Mvc;
using SP.ReportService.Models.Request;
using SP.ReportService.Models.Response;
using SP.ReportService.Service;

namespace SP.ReportService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        /// <summary>
        /// 报表服务
        /// </summary>
        private IReportServer _reportServer;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reportServer"></param>
        public ReportController(IReportServer reportServer)
        {
            _reportServer = reportServer;
        }

        /// <summary>
        /// 获取报表
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetReport")]
        public ActionResult<List<ReportResponse>> GetReport([FromBody] ReportRequest report)
        {
            var reports = _reportServer.QueryReport(report.Year, report.ReportType);
            return Ok(reports);
        }
    }
}