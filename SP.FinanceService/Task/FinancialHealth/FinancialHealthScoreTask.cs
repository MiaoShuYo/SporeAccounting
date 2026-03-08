using Quartz;
using SP.FinanceService.Service;

namespace SP.FinanceService.Task.FinancialHealth;

/// <summary>
/// 财务健康评分月度定时计算任务
/// <para>每月 1 日凌晨 2 点自动计算所有账本上月的财务健康评分</para>
/// </summary>
public class FinancialHealthScoreTask : IJob
{
    private readonly IFinancialHealthScoreService _service;
    private readonly ILogger<FinancialHealthScoreTask> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public FinancialHealthScoreTask(
        IFinancialHealthScoreService service,
        ILogger<FinancialHealthScoreTask> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// 执行月度评分计算
    /// </summary>
    public async System.Threading.Tasks.Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("财务健康评分月度任务开始执行，时间：{Time}", DateTime.Now);
        try
        {
            await _service.CalculateMonthlyScoresAsync();
            _logger.LogInformation("财务健康评分月度任务执行完成");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "财务健康评分月度任务执行失败");
            throw;
        }
    }
}
