using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SP.Common;
using SP.Common.ExceptionHandling.Exceptions;
using SP.ReportService.DB;
using SP.ReportService.Insights;
using SP.ReportService.Models.Entity;
using SP.ReportService.Models.Enumeration;
using SP.ReportService.Models.Request;
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
    private readonly ContextSession _contextSession;

    /// <summary>
    /// 报表智能解读生成器
    /// </summary>
    private readonly ReportInsightGenerator _reportInsightGenerator;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="reportServiceDbContext"></param>
    /// <param name="mapper"></param>
    /// <param name="contextSession"></param>
    /// <param name="reportInsightGenerator"></param>
    public ReportServerImpl(
        ReportServiceDBContext reportServiceDbContext,
        IMapper mapper,
        ContextSession contextSession,
        ReportInsightGenerator reportInsightGenerator)
    {
        _mapper = mapper;
        _reportServiceDbContext = reportServiceDbContext;
        _contextSession = contextSession;
        _reportInsightGenerator = reportInsightGenerator;
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

    /// <summary>
    /// 获取报表智能解读
    /// </summary>
    /// <param name="request">报表智能解读请求</param>
    /// <returns>报表智能解读</returns>
    public async Task<ReportInsightResponse> GetReportInsightAsync(ReportInsightRequest request)
    {
        var currentReports = await BuildPeriodQuery(request.Year, request.Month, request.ReportType)
            .ToListAsync();

        var previousPeriod = GetPreviousPeriod(request.Year, request.Month, request.ReportType);
        var previousReports = await BuildPeriodQuery(previousPeriod.Year, previousPeriod.Month, request.ReportType)
            .ToListAsync();

        var portrait = BuildReportPortrait(request, currentReports, previousReports);
        return await _reportInsightGenerator.GenerateAsync(portrait);
    }

    private IQueryable<Report> BuildPeriodQuery(int year, int? month, ReportTypeEnum reportType)
    {
        IQueryable<Report> query = _reportServiceDbContext.Reports
            .Where(p => p.UserId == _contextSession.UserId
                        && p.Year == year
                        && p.Type == reportType
                        && !p.IsDeleted);

        if (month.HasValue)
        {
            query = query.Where(p => p.Month == month.Value);
        }

        return query;
    }

    private static (int Year, int? Month) GetPreviousPeriod(int year, int? month, ReportTypeEnum reportType)
    {
        if (reportType == ReportTypeEnum.Month && month.HasValue)
        {
            var date = new DateTime(year, month.Value, 1).AddMonths(-1);
            return (date.Year, date.Month);
        }

        return (year - 1, month);
    }

    private static ReportInsightPortrait BuildReportPortrait(
        ReportInsightRequest request,
        List<Report> currentReports,
        List<Report> previousReports)
    {
        decimal income = currentReports.Where(IsIncomeReport).Sum(p => p.Amount);
        decimal expense = currentReports.Where(IsExpenseReport).Sum(p => p.Amount);
        decimal previousIncome = previousReports.Where(IsIncomeReport).Sum(p => p.Amount);
        decimal previousExpense = previousReports.Where(IsExpenseReport).Sum(p => p.Amount);
        decimal balance = income - expense;
        decimal previousBalance = previousIncome - previousExpense;

        var categories = currentReports
            .Where(IsExpenseReport)
            .GroupBy(p => string.IsNullOrWhiteSpace(p.Name) ? p.TransactionCategoryId.ToString() : p.Name)
            .Select(group => new ReportInsightCategoryMetric
            {
                CategoryName = group.Key,
                Amount = group.Sum(p => p.Amount),
                Percentage = expense <= 0 ? 0 : group.Sum(p => p.Amount) / expense
            })
            .OrderByDescending(p => p.Amount)
            .ToList();

        return new ReportInsightPortrait
        {
            InsightType = "MonthlyReport",
            DataPeriod = BuildDataPeriod(request.Year, request.Month, request.ReportType),
            HasData = currentReports.Count > 0,
            Income = income,
            Expense = expense,
            Balance = balance,
            PreviousIncome = previousIncome,
            PreviousExpense = previousExpense,
            PreviousBalance = previousBalance,
            IncomeChangeRate = CalculateChangeRate(income, previousIncome),
            ExpenseChangeRate = CalculateChangeRate(expense, previousExpense),
            BalanceChange = balance - previousBalance,
            Categories = categories
        };
    }

    private static string BuildDataPeriod(int year, int? month, ReportTypeEnum reportType)
    {
        return reportType == ReportTypeEnum.Month && month.HasValue
            ? $"{year}年{month.Value:D2}月"
            : $"{year}年{GetReportTypeName(reportType)}";
    }

    private static string GetReportTypeName(ReportTypeEnum reportType)
    {
        return reportType switch
        {
            ReportTypeEnum.Month => "月度报表",
            ReportTypeEnum.Quarter => "季度报表",
            ReportTypeEnum.Year => "年度报表",
            _ => "报表"
        };
    }

    private static bool IsIncomeReport(Report report)
    {
        return ContainsAny(report.Name, ["收入", "入账", "income", "revenue"]);
    }

    private static bool IsExpenseReport(Report report)
    {
        return !IsIncomeReport(report);
    }

    private static bool ContainsAny(string? text, string[] keywords)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;
        return keywords.Any(keyword => text.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    private static decimal CalculateChangeRate(decimal current, decimal previous)
    {
        return previous == 0 ? 0 : (current - previous) / previous;
    }
}