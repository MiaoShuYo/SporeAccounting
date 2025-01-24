using SporeAccounting.Models;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Server;

/// <summary>
/// 报表日志服务实现
/// </summary>
public class ReportLogImp : IReportLogServer
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly SporeAccountingDBContext _sporeAccountingDbContext;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="sporeAccountingDbContext"></param>
    public ReportLogImp(SporeAccountingDBContext sporeAccountingDbContext)
    {
        _sporeAccountingDbContext = sporeAccountingDbContext;
    }

    /// <summary>
    /// 新增报表日志
    /// </summary>
    /// <param name="reportLogs"></param>
    public void Add(List<ReportLog> reportLogs)
    {
        try
        {
            _sporeAccountingDbContext.ReportLogs.AddRange(reportLogs);
            _sporeAccountingDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 删除报表日志
    /// </summary>
    /// <param name="reportLogId"></param>
    public void Delete(string reportLogId)
    {
        try
        {
            var reportLog = _sporeAccountingDbContext.ReportLogs.FirstOrDefault(p => p.Id == reportLogId);
            if (reportLog != null)
            {
                _sporeAccountingDbContext.ReportLogs.Remove(reportLog);
                _sporeAccountingDbContext.SaveChanges();
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 修改报表日志
    /// </summary>
    /// <param name="reportLog"></param>
    public void Update(ReportLog reportLog)
    {
        try
        {
            _sporeAccountingDbContext.ReportLogs.Update(reportLog);
            _sporeAccountingDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 获取报表日志
    /// </summary>
    /// <param name="reportLogId"></param>
    /// <returns></returns>
    public ReportLog QueryByReportLog(string reportLogId)
    {
        try
        {
            var reportLog = _sporeAccountingDbContext.ReportLogs.FirstOrDefault(p => p.Id == reportLogId);
            return reportLog;
        }
        catch (Exception e)
        {
            throw;
        }
    }
    /// <summary>
    /// 获取报表日志
    /// </summary>
    /// <returns></returns>
    public List<ReportLog> Query()
    {
        try
        {
            return _sporeAccountingDbContext.ReportLogs.ToList();
        }
        catch (Exception e)
        {
            throw;
        }
    }
}