using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SP.Common;
using SP.Common.LLM;
using SP.Common.Model;
using SP.FinanceService.DB;
using SP.FinanceService.Models.Entity;
using SP.FinanceService.Models.Enumeration;
using SP.FinanceService.Models.Response;
using SP.FinanceService.Score;

namespace SP.FinanceService.Service.Impl;

/// <summary>
/// 财务健康评分服务实现类
/// </summary>
public class FinancialHealthScoreServiceImpl : IFinancialHealthScoreService
{
    private readonly FinanceServiceDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ContextSession _contextSession;
    private readonly IOpenAIService _openAIService;
    private readonly ILogger<FinancialHealthScoreServiceImpl> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public FinancialHealthScoreServiceImpl(
        FinanceServiceDbContext dbContext,
        IMapper mapper,
        ContextSession contextSession,
        IOpenAIService openAIService,
        ILogger<FinancialHealthScoreServiceImpl> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _contextSession = contextSession;
        _openAIService = openAIService;
        _logger = logger;
    }

    /// <summary>
    /// 计算并保存财务健康评分
    /// </summary>
    /// <param name="accountBookId"></param>
    /// <param name="periodStart"></param>
    /// <param name="periodEnd"></param>
    /// <returns></returns>
    public async System.Threading.Tasks.Task<FinancialHealthScoreResponse> CalculateAndSaveAsync(
        long accountBookId, DateTime periodStart, DateTime periodEnd)
    {
        long userId = _contextSession.UserId;
        var entity = await CalculateCoreAsync(accountBookId, userId, periodStart, periodEnd);
        SettingCommProperty.Create(entity);
        _dbContext.FinancialHealthScores.Add(entity);
        await _dbContext.SaveChangesAsync();
        return BuildResponse(entity);
    }

    /// <summary>
    /// 获取最新评分记录
    /// </summary>
    /// <param name="accountBookId"></param>
    /// <returns></returns>
    public FinancialHealthScoreResponse? GetLatestScore(long accountBookId)
    {
        long userId = _contextSession.UserId;
        var entity = _dbContext.FinancialHealthScores
            .Where(s => s.AccountBookId == accountBookId
                        && s.CreateUserId == userId
                        && !s.IsDeleted)
            .OrderByDescending(s => s.PeriodEnd)
            .FirstOrDefault();

        return entity == null ? null : BuildResponse(entity);
    }

    /// <summary>
    /// 获取历史评分记录（分页）
    /// </summary>
    /// <param name="accountBookId"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public PageResponse<FinancialHealthScoreResponse> GetScoreHistory(long accountBookId, int page, int size)
    {
        long userId = _contextSession.UserId;
        var query = _dbContext.FinancialHealthScores
            .Where(s => s.AccountBookId == accountBookId
                        && s.CreateUserId == userId
                        && !s.IsDeleted)
            .OrderByDescending(s => s.PeriodEnd);

        int totalCount = query.Count();
        var data = query
            .Skip((page - 1) * size)
            .Take(size)
            .ToList()
            .Select(BuildResponse)
            .ToList();

        return new PageResponse<FinancialHealthScoreResponse>
        {
            TotalCount = totalCount,
            Data = data,
            PageIndex = page,
            PageSize = size,
            TotalPage = (int)Math.Ceiling((double)totalCount / size)
        };
    }

    /// <summary>
    /// 获取当月改善建议，优先使用 LLM 生成，失败时回退到规则引擎
    /// </summary>
    /// <param name="accountBookId"></param>
    /// <returns></returns>
    public async System.Threading.Tasks.Task<List<FinancialSuggestionResponse>> GetSuggestionsAsync(long accountBookId)
    {
        var now = DateTime.Now;
        var periodStart = new DateTime(now.Year, now.Month, 1);
        var periodEnd = periodStart.AddMonths(1).AddDays(-1);

        long userId = _contextSession.UserId;

        try
        {
            // ── 1. 构建财务画像 ──
            var portrait = await FinancialPortraitBuilder.BuildAsync(
                _dbContext, accountBookId, userId, periodStart, periodEnd);

            // ── 2. 构建 Prompt ──
            var prompt = SuggestionPromptBuilder.Build(portrait);
            _logger.LogInformation("[财务健康] Prompt 已构建，长度：{Length}", prompt.Length);

            // ── 3. 调用 LLM 获取结构化建议 ──
            var suggestions = await _openAIService.ChatStructuredAsync<List<FinancialSuggestionResponse>>(prompt);

            if (suggestions != null && suggestions.Count > 0)
            {
                _logger.LogInformation(
                    "[财务健康] LLM 建议生成成功，账本 {AccountBookId}，建议数 {Count}",
                    accountBookId, suggestions.Count);
                return suggestions;
            }

            _logger.LogWarning(
                "[财务健康] LLM 返回空建议列表，账本 {AccountBookId}，降级到规则引擎",
                accountBookId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "[财务健康] LLM 调用失败，账本 {AccountBookId}，降级到规则引擎",
                accountBookId);
        }

        // ── 4. 降级：回退到规则引擎 ──
        var (income, expense, ieScore, srScore, bcScore, isScore) =
            await CalcScoreDimensionsAsync(accountBookId, userId, periodStart, periodEnd);

        _logger.LogInformation(
            "[财务健康] 规则引擎兜底建议已生成，账本 {AccountBookId}", accountBookId);

        return SuggestionEngine.Generate(income, expense, ieScore, srScore, bcScore, isScore);
    }

    /// <summary>
    /// 获取用户全部账本汇总的当月改善建议，优先使用 LLM 生成，失败时回退到规则引擎
    /// </summary>
    /// <returns></returns>
    public async System.Threading.Tasks.Task<List<FinancialSuggestionResponse>> GetSuggestionsForAllAccountBooksAsync()
    {
        var now = DateTime.Now;
        var periodStart = new DateTime(now.Year, now.Month, 1);
        var periodEnd = periodStart.AddMonths(1).AddDays(-1);

        long userId = _contextSession.UserId;

        try
        {
            // ── 1. 构建全部账本汇总的财务画像 ──
            var portrait = await FinancialPortraitBuilder.BuildForAllAccountBooksAsync(
                _dbContext, userId, periodStart, periodEnd);

            // ── 2. 构建 Prompt ──
            var prompt = SuggestionPromptBuilder.Build(portrait);
            _logger.LogInformation("[财务健康] 全账本汇总 Prompt 已构建，长度：{Length}", prompt.Length);

            // ── 3. 调用 LLM 获取结构化建议 ──
            var suggestions = await _openAIService.ChatStructuredAsync<List<FinancialSuggestionResponse>>(prompt);

            if (suggestions != null && suggestions.Count > 0)
            {
                _logger.LogInformation(
                    "[财务健康] 全账本 LLM 建议生成成功，账本数 {Count}，建议数 {SugCount}",
                    portrait.AccountBookCount, suggestions.Count);
                return suggestions;
            }

            _logger.LogWarning(
                "[财务健康] 全账本 LLM 返回空建议列表，降级到规则引擎");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "[财务健康] 全账本 LLM 调用失败，降级到规则引擎");
        }

        // ── 4. 降级：遍历各账本，汇总规则引擎建议 ──
        var allSuggestions = new List<FinancialSuggestionResponse>();
        var accountBooks = await _dbContext.AccountBooks
            .Where(ab => ab.CreateUserId == userId && !ab.IsDeleted)
            .Select(ab => ab.Id)
            .ToListAsync();

        foreach (var bookId in accountBooks)
        {
            var (income, expense, ieScore, srScore, bcScore, isScore) =
                await CalcScoreDimensionsAsync(bookId, userId, periodStart, periodEnd);
            allSuggestions.AddRange(
                SuggestionEngine.Generate(income, expense, ieScore, srScore, bcScore, isScore));
        }

        _logger.LogInformation(
            "[财务健康] 全账本规则引擎兜底建议已生成，建议数 {Count}", allSuggestions.Count);

        return allSuggestions;
    }

    /// <summary>
    /// 定时任务：计算上月财务健康评分并保存（每月 1 日凌晨 2 点执行一次）
    /// </summary>
    /// <returns></returns>
    public async System.Threading.Tasks.Task CalculateMonthlyScoresAsync()
    {
        var lastMonth = DateTime.Now.AddMonths(-1);
        var periodStart = new DateTime(lastMonth.Year, lastMonth.Month, 1);
        var periodEnd = periodStart.AddMonths(1).AddDays(-1);

        var accountBooks = await _dbContext.AccountBooks
            .Where(ab => !ab.IsDeleted)
            .Select(ab => new { ab.Id, ab.CreateUserId })
            .ToListAsync();

        foreach (var book in accountBooks)
        {
            bool exists = await _dbContext.FinancialHealthScores
                .AnyAsync(s => s.AccountBookId == book.Id
                               && s.PeriodStart == periodStart
                               && s.PeriodEnd == periodEnd
                               && !s.IsDeleted);
            if (exists) continue;

            var entity = await CalculateCoreAsync(book.Id, book.CreateUserId, periodStart, periodEnd);
            entity.Id = Snow.GetId();
            entity.CreateDateTime = DateTime.Now;
            entity.CreateUserId = book.CreateUserId;
            _dbContext.FinancialHealthScores.Add(entity);
        }

        await _dbContext.SaveChangesAsync();
    }

    // ── 私有核心方法 ────────────────────────────────────────────────

    /// <summary>
    /// 核心计算方法：计算指定账本、指定周期的财务健康评分实体（不涉及数据库持久化）
    /// </summary>
    /// <param name="accountBookId"></param>
    /// <param name="userId"></param>
    /// <param name="periodStart"></param>
    /// <param name="periodEnd"></param>
    /// <returns></returns>
    private async Task<FinancialHealthScore> CalculateCoreAsync(
        long accountBookId, long userId, DateTime periodStart, DateTime periodEnd)
    {
        var (income, expense, ieScore, srScore, bcScore, isScore) =
            await CalcScoreDimensionsAsync(accountBookId, userId, periodStart, periodEnd);

        var (totalScore, level) = ScoreCalculator.CalcTotalScore(ieScore, srScore, bcScore, isScore);

        return new FinancialHealthScore
        {
            AccountBookId = accountBookId,
            TotalScore = totalScore,
            IncomeExpenseRatioScore = ieScore,
            SavingsRateScore = srScore,
            BudgetComplianceScore = bcScore,
            IncomeStabilityScore = isScore,
            HealthLevel = level,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd
        };
    }

    /// <summary>
    /// 计算评分维度分数：收入支出、储蓄率、预算执行率、收入稳定性（供规则引擎使用）
    /// </summary>
    /// <param name="accountBookId"></param>
    /// <param name="userId"></param>
    /// <param name="periodStart"></param>
    /// <param name="periodEnd"></param>
    /// <returns></returns>
    private async System.Threading.Tasks.Task<(decimal Income, decimal Expense,
        decimal IEScore, decimal SRScore, decimal? BCScore, decimal ISScore)>
        CalcScoreDimensionsAsync(
            long accountBookId, long userId, DateTime periodStart, DateTime periodEnd)
    {
        // 加载周期内的记账条目
        var periodEntries = await LoadEntriesAsync(accountBookId, periodStart, periodEnd);

        // TransactionCategoryEnmu.Income(0) = 收入，Expenditure(1) = 支出
        decimal income = periodEntries
            .Where(e => e.Type == TransactionCategoryEnmu.Income)
            .Sum(e => e.Amount);
        decimal expense = periodEntries
            .Where(e => e.Type == TransactionCategoryEnmu.Expenditure)
            .Sum(e => e.Amount);

        // 收入稳定性：向前追溯 2 个月，共取 3 个月数据
        var stabilityStart = new DateTime(periodStart.Year, periodStart.Month, 1).AddMonths(-2);
        var stabilityEntries = await LoadEntriesAsync(accountBookId, stabilityStart, periodEnd);
        var monthlyIncomes = stabilityEntries
            .Where(e => e.Type == TransactionCategoryEnmu.Income)
            .GroupBy(e => new { e.Year, e.Month })
            .Select(g => g.Sum(x => x.Amount))
            .ToList();

        // 预算执行率
        var budgets = await _dbContext.Budgets
            .Where(b => b.CreateUserId == userId
                        && b.StartTime <= periodEnd
                        && b.EndTime >= periodStart
                        && !b.IsDeleted)
            .ToListAsync();

        var budgetItems = budgets.Select(b =>
        {
            decimal actual = periodEntries
                .Where(e => e.CategoryId == b.TransactionCategoryId
                            && e.Type == TransactionCategoryEnmu.Expenditure)
                .Sum(e => e.Amount);
            return (b.Amount, actual);
        }).ToList();

        decimal ieScore = ScoreCalculator.CalcIncomeExpenseRatioScore(income, expense);
        decimal srScore = ScoreCalculator.CalcSavingsRateScore(income, expense);
        decimal? bcScore = ScoreCalculator.CalcBudgetComplianceScore(budgetItems);
        decimal isScore = ScoreCalculator.CalcIncomeStabilityScore(monthlyIncomes);

        return (income, expense, ieScore, srScore, bcScore, isScore);
    }

    /// <summary>
    /// 加载指定账本、指定周期内的记账条目（供计算和规则引擎使用）
    /// </summary>
    /// <param name="accountBookId"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    private async System.Threading.Tasks.Task<List<AccountingEntry>> LoadEntriesAsync(
        long accountBookId, DateTime start, DateTime end)
    {
        var raw = await (
            from a in _dbContext.Accountings
            join tc in _dbContext.TransactionCategories on a.TransactionCategoryId equals tc.Id
            where a.AccountBookId == accountBookId
                  && a.RecordDate >= start
                  && a.RecordDate <= end
                  && !a.IsDeleted
                  && !tc.IsDeleted
            select new
            {
                a.AfterAmount,
                tc.Type,
                a.TransactionCategoryId,
                a.RecordDate.Year,
                a.RecordDate.Month
            }
        ).ToListAsync();

        return raw.Select(r =>
            new AccountingEntry(r.AfterAmount, r.Type, r.TransactionCategoryId, r.Year, r.Month)
        ).ToList();
    }

    /// <summary>
    /// 构建财务健康评分响应对象
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    private static FinancialHealthScoreResponse BuildResponse(FinancialHealthScore entity)
    {
        return new FinancialHealthScoreResponse
        {
            Id = entity.Id,
            AccountBookId = entity.AccountBookId,
            TotalScore = entity.TotalScore,
            IncomeExpenseRatioScore = entity.IncomeExpenseRatioScore,
            SavingsRateScore = entity.SavingsRateScore,
            BudgetComplianceScore = entity.BudgetComplianceScore,
            IncomeStabilityScore = entity.IncomeStabilityScore,
            HealthLevel = (int)entity.HealthLevel,
            HealthLevelName = GetHealthLevelName(entity.HealthLevel),
            PeriodStart = entity.PeriodStart,
            PeriodEnd = entity.PeriodEnd,
            CreateDateTime = entity.CreateDateTime
        };
    }

    /// <summary>
    /// 根据健康等级枚举值获取对应的名称
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    private static string GetHealthLevelName(HealthLevelEnum level) => level switch
    {
        HealthLevelEnum.Excellent => "优秀",
        HealthLevelEnum.Good => "良好",
        HealthLevelEnum.Fair => "一般",
        HealthLevelEnum.Poor => "较差",
        _ => "未知"
    };

    /// <summary>
    /// 记账条目内部记录（用于内存计算）
    /// </summary>
    /// <param name="Amount"></param>
    /// <param name="Type"></param>
    /// <param name="CategoryId"></param>
    /// <param name="Year"></param>
    /// <param name="Month"></param>
    private record AccountingEntry(
        decimal Amount,
        TransactionCategoryEnmu Type,
        long CategoryId,
        int Year,
        int Month);
}
