using Quartz;

namespace SP.FinanceService.Task.Accounting;

/// <summary>
/// 自动记账监控任务
/// </summary>
public class AccountingWatcher : IJob
{

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public System.Threading.Tasks.Task Execute(IJobExecutionContext context)
    {
        // 获取规则支出数据

        // 查询上次记录的执行时间

        // 如果是每天记录，并且上次执行时间小于今天，则执行记账

        // 如果是周记录，并且上次执行时间小于本周一，则执行记账

        // 如果是月记录，并且上次执行时间小于本月一号，则执行记账

        // 如果是季度记录，并且上次执行时间小于本季度第一天，则执行记账

        // 如果是年记录，并且上次执行时间小于今年一号，则执行记账

    }
}