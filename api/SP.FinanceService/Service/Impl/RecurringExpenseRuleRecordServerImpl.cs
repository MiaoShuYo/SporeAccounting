using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SP.Common.Model;
using SP.Common.Redis;
using SP.FinanceService.DB;
using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service.Impl;

/// <summary>
/// 定期支出规则记录服务实现
/// </summary>
public class RecurringExpenseRuleRecordServerImpl: IRecurringExpenseRuleRecordServer
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly FinanceServiceDbContext _dbContext;


    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dbContext"></param>
    public RecurringExpenseRuleRecordServerImpl(FinanceServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    /// <summary>
    /// 查询定期支出执行记录
    /// </summary>
    /// <param name="id">定期支出规则id</param>
    /// <returns>执行记录</returns>
    public RecurringExpenseRuleExecutionRecord GetRecordById(long id)
    {
        RecurringExpenseRuleExecutionRecord record =
            _dbContext.RecurringExpenseRuleExecutionRecords.FirstOrDefault(p => p.RecurringExpenseRuleExecutioId == id);
        return record;
    }
    /// <summary>
    /// 记录执行记录
    /// </summary>
    /// <param name="recurringExpense">执行记录</param>
    /// <returns></returns>
    public void Add(RecurringExpenseRuleExecutionRecord recurringExpense)
    {
        _dbContext.RecurringExpenseRuleExecutionRecords.Add(recurringExpense);
        _dbContext.SaveChanges();
    }
}