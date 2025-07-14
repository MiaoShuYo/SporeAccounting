using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Model;
using SP.FinanceService.DB;
using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Enumeration;
using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service.Impl;

/// <summary>
/// 预算服务器实现类
/// </summary>
public class BudgetServerImpl : IBudgetServer
{
    /// <summary>
    /// 构造函数
    /// </summary>
    private readonly FinanceServiceDbContext _dbContext;

    /// <summary>
    /// 交易分类服务器
    /// </summary>
    private readonly ITransactionCategoryServer _transactionCategoryServer;

    /// <summary>
    /// 自动映射器
    /// </summary>
    private readonly IMapper _auMapper;

    /// <summary>
    /// 预算服务器实现类构造函数
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    /// <param name="auMapper">自动映射器</param>
    /// <param name="transactionCategoryServer">交易分类服务器</param>
    public BudgetServerImpl(FinanceServiceDbContext dbContext, IMapper auMapper,
        ITransactionCategoryServer transactionCategoryServer)
    {
        _dbContext = dbContext;
        _auMapper = auMapper;
        _transactionCategoryServer = transactionCategoryServer;
    }

    /// <summary>
    /// 新增预算
    /// </summary>
    /// <param name="budget">预算</param>
    /// <returns>预算id</returns>
    public long Add(BudgetAddRequest budget)
    {
        // 预算是否存在，需要结合预算周期和预算开始时间与结束时间判断
        var existingBudget = _dbContext.Budgets
            .FirstOrDefault(b => b.TransactionCategoryId == budget.TransactionCategoryId
                                 && b.Period == budget.Period
                                 && b.StartTime <= budget.EndTime
                                 && b.EndTime >= budget.StartTime
                                 && !b.IsDeleted);

        if (existingBudget != null)
        {
            throw new BusinessException($"该分类在指定时间段内已存在预算配置");
        }

        // 映射并添加预算
        var entity = _auMapper.Map<Budget>(budget);
        entity.Remaining = budget.Amount; // 初始剩余预算等于总预算
        SettingCommProperty.Create(entity);
        _dbContext.Budgets.Add(entity);
        _dbContext.SaveChanges();
        return entity.Id;
    }

    /// <summary>
    /// 删除预算
    /// </summary>
    /// <param name="id">预算id</param>
    public void Delete(long id)
    {
        var budget = _dbContext.Budgets
            .FirstOrDefault(b => b.Id == id && !b.IsDeleted);

        if (budget == null)
        {
            throw new NotFoundException($"预算不存在，ID: {id}");
        }

        // 标记为已删除
        SettingCommProperty.Delete(budget);
        _dbContext.Budgets.Update(budget);
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// 修改预算
    /// </summary>
    /// <param name="budget">修改预算</param>
    public void Edit(BudgetEditRequest budget)
    {
        var existingBudget = _dbContext.Budgets
            .FirstOrDefault(b => b.Id == budget.Id && !b.IsDeleted);

        if (existingBudget == null)
        {
            throw new NotFoundException($"预算不存在，ID: {budget.Id}");
        }

        // 检查是否存在时间冲突的预算（排除当前预算）
        var conflictingBudget = _dbContext.Budgets
            .FirstOrDefault(b => b.Id != budget.Id
                                 && b.TransactionCategoryId == budget.TransactionCategoryId
                                 && b.Period == budget.Period
                                 && b.StartTime <= budget.EndTime
                                 && b.EndTime >= budget.StartTime
                                 && !b.IsDeleted);

        if (conflictingBudget != null)
        {
            throw new BusinessException($"该分类在指定时间段内已存在其他预算配置");
        }

        // 更新预算信息
        existingBudget.TransactionCategoryId = budget.TransactionCategoryId;
        existingBudget.Amount = budget.Amount;
        existingBudget.Period = budget.Period;
        existingBudget.Remark = budget.Remark;
        existingBudget.StartTime = budget.StartTime;
        existingBudget.EndTime = budget.EndTime;

        // 重新计算剩余预算（保持已使用金额不变）
        var usedAmount = existingBudget.Amount - existingBudget.Remaining;
        existingBudget.Remaining = budget.Amount - usedAmount;
        SettingCommProperty.Edit(existingBudget);

        _dbContext.Budgets.Update(existingBudget);
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// 查询预算分页列表
    /// </summary>
    /// <param name="request">分页查询</param>
    /// <returns>预算列表</returns>
    public PageResponse<BudgetResponse> QueryPage(BudgetPageRequest request)
    {
        var query = _dbContext.Budgets
            .Where(b => !b.IsDeleted)
            .AsQueryable();

        // 根据年份筛选
        if (request.Year > 0)
        {
            query = query.Where(b => b.StartTime.Year == request.Year);
        }

        // 根据月份筛选
        if (request.Month > 0)
        {
            query = query.Where(b => b.StartTime.Month == request.Month);
        }

        // 根据季度筛选
        if (request.Quarter > 0)
        {
            var quarterStartMonth = (request.Quarter - 1) * 3 + 1;
            var quarterEndMonth = request.Quarter * 3;
            query = query.Where(b => b.StartTime.Month >= quarterStartMonth && b.StartTime.Month <= quarterEndMonth);
        }

        // 根据周期筛选
        if (request.Period != default)
        {
            query = query.Where(b => b.Period == request.Period);
        }

        // 获取总数
        var total = query.Count();

        // 分页查询
        var budgets = query
            .OrderByDescending(b => b.CreateDateTime)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // 映射为响应模型
        var budgetResponses = _auMapper.Map<List<BudgetResponse>>(budgets);

        // 获取所有相关的交易分类ID
        var categoryIds = budgets.Select(b => b.TransactionCategoryId).Distinct().ToList();
        var categories = _dbContext.TransactionCategories
            .Where(tc => categoryIds.Contains(tc.Id) && !tc.IsDeleted)
            .ToDictionary(tc => tc.Id, tc => tc.Name);

        // 设置交易分类名称
        foreach (var response in budgetResponses)
        {
            if (categories.TryGetValue(response.TransactionCategoryId, out var categoryName))
            {
                response.TransactionCategoryName = categoryName;
            }
        }

        return new PageResponse<BudgetResponse>
        {
            Data = budgetResponses,
            TotalCount = total,
            TotalPage = (int)Math.Ceiling((double)total / request.PageSize),
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }

    /// <summary>
    /// 查询预算列表
    /// </summary>
    /// <param name="id">预算id</param>
    /// <returns>预算信息</returns>
    public BudgetResponse QueryById(long id)
    {
        var budget = _dbContext.Budgets
            .FirstOrDefault(b => b.Id == id && !b.IsDeleted);

        if (budget == null)
        {
            throw new NotFoundException($"预算不存在，ID: {id}");
        }

        // 映射为响应模型
        var response = _auMapper.Map<BudgetResponse>(budget);

        // 获取分类名称
        var category = _transactionCategoryServer.QueryById(response.TransactionCategoryId);

        if (category != null)
        {
            response.TransactionCategoryName = category.Name;
        }

        return response;
    }
}