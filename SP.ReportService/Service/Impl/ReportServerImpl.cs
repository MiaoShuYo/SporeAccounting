using AutoMapper;
using SP.Common;
using SP.Common.ExceptionHandling.Exceptions;
using SP.ReportService.DB;
using SP.ReportService.Models.Entity;
using SP.ReportService.Models.Enumeration;
using SP.ReportService.Models.Response;

namespace SP.ReportService.Service.Impl;

/// <summary>
/// 报表服务实现类
/// </summary>
public class ReportServerImpl:IReportServer
{
    /// <summary>
    /// 报表服务数据库上下文
    /// </summary>
    private readonly ReportServiceDBContext _reportServiceDbContext;
    
    /// <summary>
    /// AutoMapper实例
    /// </summary>
    private readonly IMapper _mapper;
    
    /// <summary>
    /// 上下文会话
    /// </summary>
    private ContextSession _contextSession;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="reportServiceDbContext"></param>
    /// <param name="mapper"></param>
    /// <param name="contextSession"></param>
    public ReportServerImpl(ReportServiceDBContext reportServiceDbContext, IMapper mapper, ContextSession contextSession)
    {
        _mapper = mapper;
        _reportServiceDbContext = reportServiceDbContext;
        _contextSession = contextSession;
    }
    /// <summary>
    /// 新增报表
    /// </summary>
    /// <param name="reports"></param>
    public void Add(List<Report> reports)
    {
        _reportServiceDbContext.Reports.AddRange(reports);
        _reportServiceDbContext.SaveChanges();
    }

    /// <summary>
    /// 删除报表
    /// </summary>
    public void Delete(string reportId)
    {
        if (!long.TryParse(reportId, out var reportIdValue))
        {
            throw new NotFoundException($"报表不存在，ID: {reportId}");
        }

        var report = _reportServiceDbContext.Reports
            .FirstOrDefault(p => p.Id == reportIdValue && p.UserId == _contextSession.UserId && !p.IsDeleted);
        if (report == null)
        {
            throw new NotFoundException($"报表不存在，ID: {reportId}");
        }

        _reportServiceDbContext.Reports.Remove(report);
        _reportServiceDbContext.SaveChanges();
    }
    
    /// <summary>
    /// 修改报表
    /// </summary>
    /// <param name="report"></param>
    public void Update(Report report)
    {
        var existingReport = _reportServiceDbContext.Reports
            .FirstOrDefault(p => p.Id == report.Id && p.UserId == _contextSession.UserId && !p.IsDeleted);
        if (existingReport == null)
        {
            throw new NotFoundException($"报表不存在，ID: {report.Id}");
        }

        existingReport.Year = report.Year;
        existingReport.Month = report.Month;
        existingReport.Quarter = report.Quarter;
        existingReport.Name = report.Name;
        existingReport.Type = report.Type;
        existingReport.Amount = report.Amount;
        existingReport.TransactionCategoryId = report.TransactionCategoryId;
        existingReport.UpdateDateTime = DateTime.Now;
        existingReport.UpdateUserId = _contextSession.UserId;

        _reportServiceDbContext.Reports.Update(existingReport);
        _reportServiceDbContext.SaveChanges();
    }
    
    /// <summary>
    /// 获取报表数据
    /// </summary>
    /// <param name="year"></param>
    /// <param name="reportType"></param>
    /// <returns></returns>
    public List<ReportResponse> QueryReport(int year, ReportTypeEnum reportType)
    {
        IQueryable<Report> reports = _reportServiceDbContext.Reports
            .Where(p => p.UserId == _contextSession.UserId && p.Year == year && p.Type == reportType);
        List<ReportResponse> response = _mapper.Map<List<ReportResponse>>(reports);
        return response;
    }
}