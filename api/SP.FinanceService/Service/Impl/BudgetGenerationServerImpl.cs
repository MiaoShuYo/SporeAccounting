using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Models.Enumeration;
using System.Text.Json;
using SP.Common.LLM;
using SP.Common.Redis;
using SP.Common;
using SP.Common.ExceptionHandling.Exceptions;

namespace SP.FinanceService.Service.Impl;

/// <summary>
/// 预算生成服务实现类
/// </summary>
public class BudgetGenerationServerImpl : IBudgetGenerationServer
{
    /// <summary>
    /// 预算服务接口
    /// </summary>
    private readonly IBudgetServer _budgetService;

    /// <summary>
    /// 记账服务接口
    /// </summary>
    private readonly IAccountingServer _accountingServer;

    /// <summary>
    /// 记账分类服务接口
    /// </summary>
    private readonly ITransactionCategoryServer _transactionCategoryServer;

    /// <summary>
    /// 大模型服务
    /// </summary>
    private readonly IOpenAIService _openAIService;

    /// <summary>
    /// Redis缓存服务接口
    /// </summary>
    private readonly IRedisService _redisService;

    /// <summary>
    /// 上下文服务
    /// </summary>
    private readonly ContextSession _contextSession;

    /// <summary>
    // redis缓存键前缀
    // </summary>
    private const string RedisCacheKeyPrefix = "budget_generation_preview:";


    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="budgetService">预算服务接口</param>
    /// <param name="accountingServer">记账服务接口</param>
    /// <param name="transactionCategoryServer">记账分类服务接口</param>
    /// <param name="openAIService">大模型服务接口</param>
    /// <param name="redisService">Redis缓存服务接口</param>
    /// <param name="contextSession">上下文服务接口</param>
    public BudgetGenerationServerImpl(IBudgetServer budgetService, IAccountingServer accountingServer, ITransactionCategoryServer transactionCategoryServer, IOpenAIService openAIService, IRedisService redisService, ContextSession contextSession)
    {
        _budgetService = budgetService;
        _accountingServer = accountingServer;
        _transactionCategoryServer = transactionCategoryServer;
        _openAIService = openAIService;
        _redisService = redisService;
        _contextSession = contextSession;
    }

    /// <summary>
    /// AI生成预算，返回预览数据
    /// </summary>
    /// <param name="request">生成请求</param>
    /// <returns>预览数据</returns>
    public async Task<List<BudgetGenerationResponse>> GenerateBudget(BudgetGenerationRequest request)
    {
        // 1. 构建AI输入数据
        var aiInputData = BuildAiInputData();
        if (aiInputData == null)
            return new List<BudgetGenerationResponse>();

        // 2. 获取预算周期信息
        var (month, beginDate, endDate) = GetBudgetPeriodInfo(request.Period);

        // 3. 构建提示词并调用AI服务
        var prompt = BuildBudgetPrompt(aiInputData, month, beginDate, endDate);
        var budgetResponses = await _openAIService.ChatStructuredAsync<List<BudgetGenerationResponse>>(prompt);

        // 4. 缓存预览数据
        await CacheBudgetPreviewAsync(budgetResponses);

        // 5. 返回预览数据
        return budgetResponses ?? new List<BudgetGenerationResponse>();
    }


    /// <summary>
    /// 确认/取消生成预算，返回生成结果
    /// </summary>
    /// <param name="id">AI生成预算id</param>
    /// <param name="confirm">是否确认生成</param>
    /// <returns>任务</returns>
    public async System.Threading.Tasks.Task ConfirmBudget(long id, bool confirm)
    {
        // 查询缓存中的预算预览数据
        var cacheKey = $"{RedisCacheKeyPrefix}{_contextSession.UserId}";
        var cachedData = await _redisService.GetAsync<List<BudgetGenerationResponse>>(cacheKey);
        if (cachedData == null)
        {
            throw new BusinessException("未找到预算预览数据，请重新生成预算");
        }
        if (!confirm)
        {
            // 取消生成，删除缓存并返回0
            await _redisService.RemoveAsync(cacheKey);
            return;
        }
        // 确认生成，将预览数据保存为正式预算
        var budgetAdds = new List<BudgetAddRequest>();

        foreach (var item in cachedData)
        {
            budgetAdds.Add(new BudgetAddRequest
            {
                TransactionCategoryId = item.TransactionCategoryId,
                Amount = item.Amount,
                Period = (PeriodEnum)item.Period,
                Remark = item.Remark,
                StartTime = item.StartTime,
                EndTime = item.EndTime
            });
        }
        await _budgetService.Adds(budgetAdds);
    }

    /// <summary>
    /// 构建AI生成预算所需的输入数据
    /// </summary>
    /// <returns>输入数据，若无数据则返回null</returns>
    private object? BuildAiInputData()
    {
        // 查询用户最近12个月的记账数据
        var accountingResponses = _accountingServer.GetAccountingsByTimeRange(
            DateTime.Now.AddMonths(-12), DateTime.Now);
        if (accountingResponses == null)
            return null;

        // 获取支出分类数据
        var categoryResponses = _transactionCategoryServer.QueryByType(TransactionCategoryEnmu.Expenditure);
        if (categoryResponses == null)
            return null;

        var categoryIds = categoryResponses.Select(c => c.Id).ToList();

        // 过滤出支出的记账数据
        var categorySummaries = accountingResponses
            .Where(a => categoryIds.Contains(a.TransactionCategoryId));

        // 按支出分类分组，计算每个支出分类的总额
        var categoryTotalAmounts = categorySummaries
            .GroupBy(a => a.TransactionCategoryId)
            .Select(g => new
            {
                CategoryId = g.Key,
                TotalAmount = g.Sum(a => a.Amount)
            })
            .ToList();

        // 按支出分类和月份分组，计算每个月的支出分类总额
        var monthlyCategoryAmounts = categorySummaries
            .GroupBy(a => new { a.TransactionCategoryId, Month = a.RecordDate.Month })
            .Select(g => new
            {
                CategoryId = g.Key.TransactionCategoryId,
                Month = g.Key.Month,
                TotalAmount = g.Sum(a => a.Amount)
            })
            .ToList();

        // 组装为AI输入格式
        return categoryTotalAmounts.Select(c => new
        {
            CategoryId = c.CategoryId,
            CategoryName = categoryResponses.FirstOrDefault(r => r.Id == c.CategoryId)?.Name,
            TotalAmount = c.TotalAmount,
            MonthlyAmounts = monthlyCategoryAmounts
                .Where(m => m.CategoryId == c.CategoryId)
                .Select(m => new { m.Month, m.TotalAmount })
                .ToList()
        }).ToList();
    }

    /// <summary>
    /// 根据预算周期获取月份数、起始日期和结束日期
    /// </summary>
    /// <param name="period">预算周期</param>
    /// <returns>月份数、起始日期、结束日期</returns>
    private static (int month, DateTime beginDate, DateTime endDate) GetBudgetPeriodInfo(PeriodEnum period)
    {
        var beginDate = DateTime.Now;
        var endDate = period switch
        {
            PeriodEnum.Month => DateTime.Now.AddMonths(1),
            PeriodEnum.Quarter => DateTime.Now.AddMonths(3),
            PeriodEnum.Year => DateTime.Now.AddMonths(12),
            _ => DateTime.Now
        };

        var month = period switch
        {
            PeriodEnum.Month => 1,
            PeriodEnum.Quarter => 3,
            PeriodEnum.Year => 12,
            _ => 0
        };

        return (month, beginDate, endDate);
    }

    /// <summary>
    /// 构建AI生成预算的提示词
    /// </summary>
    /// <param name="aiInputData">AI输入数据</param>
    /// <param name="month">预算月数</param>
    /// <param name="beginDate">预算起始日期</param>
    /// <param name="endDate">预算结束日期</param>
    /// <returns>提示词字符串</returns>
    private static string BuildBudgetPrompt(object aiInputData, int month, DateTime beginDate, DateTime endDate)
    {
        return $"请根据以下数据生成未来{month}个月的预算，预算起止日期为{beginDate:yyyy-MM-dd}至{endDate:yyyy-MM-dd}，要求按照支出分类进行预算分配，并且给出每个支出分类的预算金额：{JsonSerializer.Serialize(aiInputData)}";
    }

    /// <summary>
    /// 将生成的预算预览数据缓存到Redis，过期时间30分钟
    /// </summary>
    /// <param name="data">预算预览数据</param>
    private async System.Threading.Tasks.Task CacheBudgetPreviewAsync(List<BudgetGenerationResponse> data)
    {
        var cacheKey = $"{RedisCacheKeyPrefix}{_contextSession.UserId}";
        await _redisService.SetAsync(cacheKey, JsonSerializer.Serialize(data), 30 * 60);
    }
}