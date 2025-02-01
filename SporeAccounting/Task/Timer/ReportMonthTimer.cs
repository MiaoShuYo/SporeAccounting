using Quartz;
using SporeAccounting.Models;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Task.Timer;

/// <summary>
/// 月度报表定时器
/// </summary>
public class ReportMonthTimer : IJob
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serviceScopeFactory"></param>
    public ReportMonthTimer(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public System.Threading.Tasks.Task Execute(IJobExecutionContext context)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        // 获取每个用户最近一次报表记录日期
        var reportServer = scope.ServiceProvider.GetRequiredService<IReportServer>();
        var incomeExpenditureRecordServer = scope.ServiceProvider.GetRequiredService<IIncomeExpenditureRecordServer>();
        var reportLogServer = scope.ServiceProvider.GetRequiredService<IReportLogServer>();
        var reportLogs = reportLogServer.Query();
        var reportLogDic = reportLogs
            .GroupBy(x => x.UserId)
            .ToDictionary(x => x.Key,
                x => x.Max(x => x.CreateDateTime));
        // 查询上次日期以后的记账记录
        List<Report> dbReports = new();
        List<ReportLog> dbReportLogs = new();
        foreach (var log in reportLogDic)
        {
            var incomeExpenditureRecords = incomeExpenditureRecordServer
                .QueryByUserId(log.Key);
            incomeExpenditureRecords = incomeExpenditureRecords
                .Where(x => x.RecordDate > log.Value)
                .Where(p => p.IncomeExpenditureClassification.Type == IncomeExpenditureTypeEnmu.Income).ToList();
            // 生成报表
            // 按照月度创建报表数据，根据支出类型统计
            var monthlyReports = incomeExpenditureRecords
                .GroupBy(x => new { x.RecordDate.Year, x.RecordDate.Month })
                .Select(g => new Report
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Name = $"{g.Key.Year}年{g.Key.Month}月报表",
                    Type = ReportTypeEnum.Month,
                    Amount = g.Sum(x => x.AfterAmount),
                    UserId = log.Key,
                    ClassificationId = g.First().IncomeExpenditureClassificationId,
                    CreateDateTime = DateTime.Now,
                    CreateUserId = log.Key
                }).ToList();
            dbReports.AddRange(monthlyReports);

            // 记录日志
            var reportLogEntries = dbReports.Select(report => new ReportLog
            {
                UserId = report.UserId,
                ReportId = report.Id,
                CreateDateTime = DateTime.Now,
                CreateUserId = report.UserId
            }).ToList();
            dbReportLogs.AddRange(reportLogEntries);

            // 保存报表和日志到数据库
            reportServer.Add(dbReports);
            reportLogServer.Add(dbReportLogs);
        }


        return System.Threading.Tasks.Task.CompletedTask;
    }
}