using AutoMapper;
using SP.Common;
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
        var report = _reportServiceDbContext.Reports.Find(reportId);
        _reportServiceDbContext.Reports.Remove(report);
        _reportServiceDbContext.SaveChanges();
    }
    
    /// <summary>
    /// 修改报表
    /// </summary>
    /// <param name="report"></param>
    public void Update(Report report)
    {
        _reportServiceDbContext.Reports.Update(report);
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