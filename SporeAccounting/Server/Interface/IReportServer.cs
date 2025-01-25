using SporeAccounting.Models;

namespace SporeAccounting.Server.Interface;

/// <summary>
/// 报表服务
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
    /// <param name="userId"></param>
    /// <param name="year"></param>
    /// <param name="reportType"></param>
    /// <returns></returns>
    List<Report> QueryReport(string userId, int year,ReportTypeEnum reportType);
}