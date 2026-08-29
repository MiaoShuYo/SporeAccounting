using SP.ReportService.Models.Entity;
using SP.ReportService.Models.Enumeration;
using SP.ReportService.Models.Request;
using SP.ReportService.Models.Response;

namespace SP.ReportService.Service;

/// <summary>
/// 报表服务接口
/// </summary>
public interface IReportServer
{
    /// <summary>
    /// 新增报表
    /// </summary>
    /// <param name="reports"></param>
    /// <returns></returns>
    void Add(List<Report> reports);
    
    /// <summary>
    /// 删除报表
    /// </summary>
    /// <param name="reportId"></param>
    /// <returns></returns>
    void Delete(string reportId);
    
    /// <summary>
    /// 修改报表
    /// </summary>
    /// <param name="report"></param>
    /// <returns></returns>
    void Update(Report report);
    
    /// <summary>
    /// 获取报表数据
    /// </summary>
    /// <param name="year"></param>
    /// <param name="reportType"></param>
    /// <returns></returns>
    List<ReportResponse> QueryReport(int year,ReportTypeEnum reportType);

    /// <summary>
    /// 获取报表智能解读
    /// </summary>
    /// <param name="request">报表智能解读请求</param>
    /// <returns>报表智能解读</returns>
    Task<ReportInsightResponse> GetReportInsightAsync(ReportInsightRequest request);
}