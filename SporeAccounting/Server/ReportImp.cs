using SporeAccounting.Models;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Server;

/// <summary>
/// 报表服务实现
/// </summary>
public class ReportImp : IReportServer
{
    private readonly SporeAccountingDBContext _sporeAccountingDbContext;

    public ReportImp(SporeAccountingDBContext sporeAccountingDbContext)
    {
        _sporeAccountingDbContext = sporeAccountingDbContext;
    }

    /// <summary>
    /// 添加报表
    /// </summary>
    /// <param name="report"></param>
    public void Add(Report report)
    {
        try
        {
            _sporeAccountingDbContext.Reports.Add(report);
            _sporeAccountingDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 删除报表
    /// </summary>
    /// <param name="reportId"></param>
    public void Delete(string reportId)
    {
        try
        {
            var report = _sporeAccountingDbContext.Reports.Find(reportId);
            _sporeAccountingDbContext.Reports.Remove(report);
            _sporeAccountingDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 修改报表
    /// </summary>
    /// <param name="report"></param>
    public void Update(Report report)
    {
        try
        {
            _sporeAccountingDbContext.Reports.Update(report);
            _sporeAccountingDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 获取报表数据
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    public List<Report> QueryReport(string userId, int year)
    {
        try
        {
            IQueryable<Report> reports = _sporeAccountingDbContext.Reports
                .Where(p => p.UserId == userId && p.Year == year);

            return reports.ToList();
        }
        catch (Exception e)
        {
            throw;
        }
    }
}