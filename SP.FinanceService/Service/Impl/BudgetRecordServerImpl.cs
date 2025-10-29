using AutoMapper;
using SP.FinanceService.DB;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service.Impl;

/// <summary>
/// 预算记录服务实现类
/// </summary>
public class BudgetRecordServerImpl : IBudgetRecordServer
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
    /// 预算服务
    /// </summary>
    private readonly IBudgetServer _budgetServer;

    /// <summary>
    /// 预算记录服务构造函数
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="automapper"></param>
    /// <param name="budgetServer"></param>
    public BudgetRecordServerImpl(FinanceServiceDbContext dbContext, IMapper automapper, IBudgetServer budgetServer)
    {
        _dbContext = dbContext;
        _automapper = automapper;
        _budgetServer = budgetServer;
    }

    /// <summary>
    /// 根据预算Id集合获取预算记录
    /// </summary>
    /// <returns>预算记录集合</returns>
    public Dictionary<long, List<BudgetRecordResponse>> GetBudgetRecordsByBudgetIds()
    {
        // 获取在用的预算Id集合
        var budgets = _budgetServer.QueryActiveBudgets();
        var budgetIds = budgets.Select(b => b.Id).ToList();
        // 查询预算记录
        var budgetRecords = _dbContext.BudgetRecords
            .Where(br => budgetIds.Contains(br.BudgetId))
            .ToList();
        // 将实体映射到响应模型
        var budgetRecordResponses = _automapper.Map<List<BudgetRecordResponse>>(budgetRecords);
        // 设置预算周期和预算类型
        for (int i = 0; i < budgetRecordResponses.Count; i++)
        {
            var budget = budgets.FirstOrDefault(b => b.Id == budgetRecordResponses[i].BudgetId);
            if (budget != null)
            {
                budgetRecordResponses[i].Period = budget.Period;
                budgetRecordResponses[i].TransactionCategoryId = budget.TransactionCategoryId;
            }
        }

        // 按预算Id分组
        var groupedRecords = budgetRecordResponses
            .GroupBy(br => br.BudgetId)
            .ToDictionary(g => g.Key, g => g.ToList());
        return groupedRecords;
    }
}