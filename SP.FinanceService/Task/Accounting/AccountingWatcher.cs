using System.Threading.Tasks;
using Quartz;
using SP.Common;
using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Enumeration;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Service;
using Twilio.Http;

namespace SP.FinanceService.Task.Accounting;

/// <summary>
/// 自动记账监控任务
/// </summary>
public class AccountingWatcher : IJob
{
    /// <summary>
    /// 定期支出规则服务
    /// </summary>
    private readonly IRecurringExpenseRuleServer _recurringExpenseRuleServer;

    /// <summary>
    /// 定期支出规则记录服务
    /// </summary>
    private readonly IRecurringExpenseRuleRecordServer _recurringExpenseRuleRecordServer;

    /// <summary>
    /// 记账服务
    /// </summary>
    private readonly IAccountingServer _accountingServer;


    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="recurringExpenseRuleServer"></param>
    /// <param name="recurringExpenseRuleRecordServer"></param>
    /// <param name="accountingServer"></param>
    public AccountingWatcher(IRecurringExpenseRuleServer recurringExpenseRuleServer,
        IRecurringExpenseRuleRecordServer recurringExpenseRuleRecordServer, IAccountingServer accountingServer)
    {
        _recurringExpenseRuleServer = recurringExpenseRuleServer;
        _recurringExpenseRuleRecordServer = recurringExpenseRuleRecordServer;
        _accountingServer = accountingServer;
    }

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async System.Threading.Tasks.Task Execute(IJobExecutionContext context)
    {
        // 获取规则支出数据
        List<RecurringExpenseRuleResponse> recurringExpenseRules =
            await _recurringExpenseRuleServer.GetAllRecurringExpenseRules();

        foreach (var recurringExpense in recurringExpenseRules)
        {
            // 查询上次记录的执行时间
            RecurringExpenseRuleExecutionRecord record =
                _recurringExpenseRuleRecordServer.GetRecordById(recurringExpense.Id);

            // 如果是每天记录，并且上次执行时间小于今天，则执行记账
            if (recurringExpense.Frequency == FrequencyEnum.Day)
            {
                if (record == null || record.CreateDateTime.Date < DateTime.Now.Date)
                {
                    await Account(recurringExpense);
                    RecurringExpenseRuleExecutionRecord newRecord = new RecurringExpenseRuleExecutionRecord();
                    newRecord.IsOK = true;
                    newRecord.RecurringExpenseRuleExecutioId = recurringExpense.Id;
                    newRecord.CreateDateTime = DateTime.Now;
                    newRecord.CreateUserId = recurringExpense.CreateUserId;
                    newRecord.Id = Snow.GetId();
                    _recurringExpenseRuleRecordServer.Add(newRecord);
                }

                continue;
            }

            // 如果是周记录，并且上次执行时间小于本周一，则执行记账
            if (recurringExpense.Frequency == FrequencyEnum.Week)
            {
                if (record == null || record.CreateDateTime < GetStartOfWeek(DateTime.Now))
                {
                    await Account(recurringExpense);

                    RecurringExpenseRuleExecutionRecord newRecord = new RecurringExpenseRuleExecutionRecord();
                    newRecord.IsOK = true;
                    newRecord.RecurringExpenseRuleExecutioId = recurringExpense.Id;
                    newRecord.CreateDateTime = DateTime.Now;
                    newRecord.CreateUserId = recurringExpense.CreateUserId;
                    newRecord.Id = Snow.GetId();
                    _recurringExpenseRuleRecordServer.Add(newRecord);
                }

                continue;
            }

            // 如果是月记录，并且上次执行时间小于本月一号，则执行记账
            if (recurringExpense.Frequency == FrequencyEnum.Month)
            {
                if (record == null ||
                    record.CreateDateTime.Date < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
                {
                    await Account(recurringExpense);
                    RecurringExpenseRuleExecutionRecord newRecord = new RecurringExpenseRuleExecutionRecord();
                    newRecord.IsOK = true;
                    newRecord.RecurringExpenseRuleExecutioId = recurringExpense.Id;
                    newRecord.CreateDateTime = DateTime.Now;
                    newRecord.CreateUserId = recurringExpense.CreateUserId;
                    newRecord.Id = Snow.GetId();
                    _recurringExpenseRuleRecordServer.Add(newRecord);
                }

                continue;
            }

            // 如果是季度记录，并且上次执行时间小于本季度第一天，则执行记账
            if (recurringExpense.Frequency == FrequencyEnum.Quarter)
            {
                if(record==null || record.CreateDateTime.Date < new DateTime(DateTime.Now.Year, (DateTime.Now.Month - 1) / 3 * 3 + 1, 1))
                {
                    await Account(recurringExpense);
                    RecurringExpenseRuleExecutionRecord newRecord = new RecurringExpenseRuleExecutionRecord();
                    newRecord.IsOK = true;
                    newRecord.RecurringExpenseRuleExecutioId = recurringExpense.Id;
                    newRecord.CreateDateTime = DateTime.Now;
                    newRecord.CreateUserId = recurringExpense.CreateUserId;
                    newRecord.Id = Snow.GetId();
                    _recurringExpenseRuleRecordServer.Add(newRecord);
                }

                continue;
            }

            // 如果是年记录，并且上次执行时间小于今年一号，则执行记账
            if (recurringExpense.Frequency == FrequencyEnum.Year)
            {
                if (record == null || record.CreateDateTime.Date < new DateTime(DateTime.Now.Year, 1, 1))
                {
                    await Account(recurringExpense);
                    RecurringExpenseRuleExecutionRecord newRecord = new RecurringExpenseRuleExecutionRecord();
                    newRecord.IsOK = true;
                    newRecord.RecurringExpenseRuleExecutioId = recurringExpense.Id;
                    newRecord.CreateDateTime = DateTime.Now;
                    newRecord.CreateUserId = recurringExpense.CreateUserId;
                    newRecord.Id = Snow.GetId();
                    _recurringExpenseRuleRecordServer.Add(newRecord);
                }
                continue;
            }
        }
    }

    /// <summary>
    /// 记账
    /// </summary>
    /// <param name="recurringExpense"></param>
    private async System.Threading.Tasks.Task Account(RecurringExpenseRuleResponse recurringExpense)
    {
        long accountBookId = recurringExpense.AccountBookId;
        AccountingAddRequest accountingAdd = new AccountingAddRequest();
        accountingAdd.AccountBookId = accountBookId;
        accountingAdd.Amount = recurringExpense.Amount;
        accountingAdd.CurrencyId = recurringExpense.CurrencyId;
        accountingAdd.RecordDate = DateTime.Now;
        accountingAdd.TransactionCategoryId = recurringExpense.CategoryId;
        await _accountingServer.Add(accountBookId, accountingAdd);
    }

    /// <summary>
    /// 获取本周第一天(周一)
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>本周第一天</returns>
    private DateTime GetStartOfWeek(DateTime date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }
}