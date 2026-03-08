using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SP.Common;
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

    /// <summary>
    /// 构造函数
    /// </summary>
    public FinancialHealthScoreServiceImpl(
        FinanceServiceDbContext dbContext,
        IMapper mapper,
        ContextSession contextSession)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _contextSession = contextSession;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public async System.Threading.Tasks.Task<List<FinancialSuggestionResponse>> GetSuggestionsAsync(long accountBookId)
    {
        var now = DateTime.Now;
        var periodStart = new DateTime(now.Year, now.Month, 1);
        var periodEnd = periodStart.AddMonths(1).AddDays(-1);

        long userId = _contextSession.UserId;
        var (income, expense, ieScore, srScore, bcScore, isScore) =
            await CalcScoreDimensionsAsync(accountBookId, userId, periodStart, periodEnd);

        return SuggestionEngine.Generate(income, expense, ieScore, srScore, bcScore, isScore);
    }

    /// <inheritdoc/>
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
    /// 核心计算：加载数据并构建评分实体（不保存）
    /// </summary>
    private async System.Threading.Tasks.Task<FinancialHealthScore> CalculateCoreAsync(
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
    /// 计算各维度得分原始数值
    /// </summary>
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
    /// 从数据库加载指定账本、时间段内的记账条目
    /// </summary>
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
    /// 将实体转换为响应模型
    /// </summary>
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
    private record AccountingEntry(
        decimal Amount,
        TransactionCategoryEnmu Type,
        long CategoryId,
        int Year,
        int Month);
}
