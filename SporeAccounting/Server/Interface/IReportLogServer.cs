using SporeAccounting.Models;

namespace SporeAccounting.Server.Interface;
/// <summary>
/// 报表日志服务接口
/// </summary>
public interface IReportLogServer
{
    /// <summary>
    /// 新增报表日志
    /// </summary>
    /// <param name="reportLogs"></param>
    void Add(List<ReportLog> reportLogs);

    /// <summary>
    /// 删除报表日志
    /// </summary>
    /// <param name="reportLogId"></param>
    void Delete(string reportLogId);

    /// <summary>
    /// 修改报表日志
    /// </summary>
    /// <param name="reportLog"></param>
    void Update(ReportLog reportLog);

    /// <summary>
    /// 查询报表日志
    /// </summary>
    /// <param name="reportLogId"></param>
    /// <returns></returns>
    ReportLog QueryByReportLog(string reportLogId);

    /// <summary>
    /// 分页查询报表日志
    /// </summary>
    /// <returns></returns>
    List<ReportLog> Query();
}