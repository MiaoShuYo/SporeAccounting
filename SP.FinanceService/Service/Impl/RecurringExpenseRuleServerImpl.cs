using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Model;
using SP.Common.Redis;
using SP.FinanceService.DB;
using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Enumeration;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service.Impl;

public class RecurringExpenseRuleServerImpl : IRecurringExpenseRuleServer
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
    /// redis 客户端
    /// </summary>
    private readonly IRedisService _redisService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="automapper"></param>
    /// <param name="redisService"></param>
    public RecurringExpenseRuleServerImpl(FinanceServiceDbContext dbContext, IMapper automapper,
        IRedisService redisService)
    {
        _dbContext = dbContext;
        _automapper = automapper;
        _redisService = redisService;
    }


    /// <summary>
    /// 添加定期支出规则
    /// </summary>
    /// <param name="recurringExpenseRuleAdd">定期支出参数</param>
    /// <returns></returns>
    public async Task<long> AddRecurringExpenseRule(RecurringExpenseRuleAddRequest recurringExpenseRuleAdd)
    {
        RecurringExpenseRule recurringExpenseRule = _automapper.Map<RecurringExpenseRule>(recurringExpenseRuleAdd);
        SettingCommProperty.Create(recurringExpenseRule);
        _dbContext.RecurringExpenseRules.Add(recurringExpenseRule);
        await _dbContext.SaveChangesAsync();
        // 清理redis 缓存
        string recurringExpenseKey = FinanceRedisKey.RecurringExpenseKey;
        await _redisService.RemoveAsync(recurringExpenseKey);
        string userRecurringExpenseKey =
            string.Format(FinanceRedisKey.RecurringExpenseUserKey, recurringExpenseRule.CreateUserId);
        await _redisService.RemoveAsync(userRecurringExpenseKey);
        return recurringExpenseRule.Id;
    }

    /// <summary>
    /// 修改定期支出规则
    /// </summary>
    /// <param name="recurringExpenseRuleEdit">定期支出参数</param>
    /// <returns></returns>
    public async Task<long> EditRecurringExpenseRule(RecurringExpenseRuleEditRequest recurringExpenseRuleEdit)
    {
        RecurringExpenseRule recurringExpenseRule =
            _dbContext.RecurringExpenseRules.FirstOrDefault(p => p.Id == recurringExpenseRuleEdit.Id);
        if (recurringExpenseRule == null)
        {
            throw new NotFoundException("定期指出规则不存在");
        }

        _automapper.Map(recurringExpenseRuleEdit, recurringExpenseRule);
        SettingCommProperty.Edit(recurringExpenseRule);
        await _dbContext.SaveChangesAsync();
        // 清理redis 缓存
        string recurringExpenseKey = FinanceRedisKey.RecurringExpenseKey;
        await _redisService.RemoveAsync(recurringExpenseKey);
        string userRecurringExpenseKey =
            string.Format(FinanceRedisKey.RecurringExpenseUserKey, recurringExpenseRule.CreateUserId);
        await _redisService.RemoveAsync(userRecurringExpenseKey);
        return recurringExpenseRule.Id;
    }

    /// <summary>
    /// 删除定期支出规则
    /// </summary>
    /// <param name="ids">定期支出规则id集合</param>
    public async System.Threading.Tasks.Task DeleteRecurringExpenseRule(List<long> ids)
    {
        List<RecurringExpenseRule> recurringExpenseRules = _dbContext.RecurringExpenseRules.Where(p => ids.Contains(p.Id)).ToList();
        if (recurringExpenseRules == null)
        {
            throw new NotFoundException("定期指出规则不存在");
        }

        foreach (var recurringExpenseRule in recurringExpenseRules)
        {
            SettingCommProperty.Delete(recurringExpenseRule);
        }

        string recurringExpenseKey = FinanceRedisKey.RecurringExpenseKey;
        await _redisService.RemoveAsync(recurringExpenseKey);
        await _dbContext.SaveChangesAsync();
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
            return null;
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
            PageIndex = page.PageIndex,
            PageSize = page.PageSize,
            TotalPage = (int)Math.Ceiling((double)total / page.PageSize) * page.PageSize
        };
    }

    /// <summary>
    /// 获取全部定期支出规则
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<List<RecurringExpenseRuleResponse>> GetAllRecurringExpenseRules()
    {
        // 查询redis
        string recurringExpenseKey = FinanceRedisKey.RecurringExpenseKey;
        List<RecurringExpenseRuleResponse> data =
            await _redisService.GetAsync<List<RecurringExpenseRuleResponse>>(recurringExpenseKey);
        if (data != null)
        {
            return data;
        }

        var dbDate = await  _dbContext.RecurringExpenseRules.Where(p => !p.IsDeleted).ToListAsync();
        data = _automapper.Map<List<RecurringExpenseRuleResponse>>(dbDate);

        // 记录redis
        await _redisService.SetAsync(recurringExpenseKey, data);
        return data;
    }
}