using AutoMapper;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Model;
using SP.FinanceService.DB;
using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service.Impl;

public class RecurringExpenseRuleServerImpl: IRecurringExpenseRuleServer
{

    /// <summary>
    /// 数据库上下文
    /// </summary>
    private readonly FinanceServiceDbContext _dbContext;

    /// <summary>
    /// 自动映射器
    /// </summary>
    private readonly IMapper _automapper;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="automapper"></param>
    public RecurringExpenseRuleServerImpl(FinanceServiceDbContext dbContext, IMapper automapper)
    {
        _dbContext = dbContext;
        _automapper = automapper;
    }


    /// <summary>
    /// 添加定期支出规则
    /// </summary>
    /// <param name="recurringExpenseRuleAdd">定期支出参数</param>
    /// <returns></returns>
    public long AddRecurringExpenseRule(RecurringExpenseRuleAddRequest recurringExpenseRuleAdd)
    {
        RecurringExpenseRule recurringExpenseRule = _automapper.Map<RecurringExpenseRule>(recurringExpenseRuleAdd);
        SettingCommProperty.Create(recurringExpenseRule);
        _dbContext.RecurringExpenseRules.Add(recurringExpenseRule);
        _dbContext.SaveChanges();
        return recurringExpenseRule.Id;
    }
    /// <summary>
    /// 修改定期支出规则
    /// </summary>
    /// <param name="recurringExpenseRuleEdit">定期支出参数</param>
    /// <returns></returns>
    public long EditRecurringExpenseRule(RecurringExpenseRuleEditRequest recurringExpenseRuleEdit)
    {
        RecurringExpenseRule recurringExpenseRule = _dbContext.RecurringExpenseRules.FirstOrDefault(p => p.Id == recurringExpenseRuleEdit.Id);
        if (recurringExpenseRule == null)
        {
            throw new NotFoundException("定期指出规则不存在");
        }
        _automapper.Map(recurringExpenseRuleEdit, recurringExpenseRule);
        SettingCommProperty.Edit(recurringExpenseRule);
        _dbContext.SaveChanges();
        return recurringExpenseRule.Id;

    }
    /// <summary>
    /// 删除定期支出规则
    /// </summary>
    /// <param name="id">定期支出规则id</param>
    public void DeleteRecurringExpenseRule(long id)
    {
        RecurringExpenseRule recurringExpenseRule = _dbContext.RecurringExpenseRules.FirstOrDefault(p => p.Id == id);
        if (recurringExpenseRule == null)
        {
            throw new NotFoundException("定期指出规则不存在");
        }

        SettingCommProperty.Delete(recurringExpenseRule);
        _dbContext.SaveChanges();

    }
    /// <summary>
    /// 获取定期支出规则
    /// </summary>
    /// <param name="id">定期支出规则id</param>
    /// <returns></returns>
    public RecurringExpenseRuleResponse GetRecurringExpenseRule(long id)
    {
        RecurringExpenseRule recurringExpenseRule = _dbContext.RecurringExpenseRules.FirstOrDefault(p => p.Id == id);
        if (recurringExpenseRule == null)
        {
            throw new NotFoundException("定期指出规则不存在");
        }

        RecurringExpenseRuleResponse response = _automapper.Map<RecurringExpenseRuleResponse>(recurringExpenseRule);
        return response;
    }
    /// <summary>
    /// 分页查询定期支出规则
    /// </summary>
    /// <param name="page">分页参数</param>
    /// <returns></returns>
    public PageResponse<RecurringExpenseRuleResponse> GetRecurringExpenseRulePage(RecurringExpenseRulePgRequest page)
    {
        var query = _dbContext.RecurringExpenseRules.AsQueryable();
        if (!string.IsNullOrEmpty(page.Title))
        {
            query = query.Where(p => p.Title.Contains(page.Title));
        }
        int total = query.Count();
        var items = query
            .Skip((page.PageIndex - 1) * page.PageSize)
            .Take(page.PageSize)
            .ToList();
        var responseItems = _automapper.Map<List<RecurringExpenseRuleResponse>>(items);
        return new PageResponse<RecurringExpenseRuleResponse>
        {
            TotalCount = total,
            Data = responseItems,
            PageIndex=page.PageIndex,
            PageSize=page.PageSize,
            TotalPage= (int)Math.Ceiling((double)total / page.PageSize) * page.PageSize
        };
    }
}