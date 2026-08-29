using Microsoft.EntityFrameworkCore;
using SP.FinanceService.DB;
using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Score;

/// <summary>
/// 财务画像数据构建器
/// 从数据库采集多维原始财务数据，构建 LLM 可理解的财务画像
/// <para>支持两种模式：单个账本分析 / 用户全部账本汇总分析</para>
/// </summary>
public static class FinancialPortraitBuilder
{
    /// <summary>
    /// 构建单个账本的财务画像
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    /// <param name="accountBookId">账本 ID</param>
    /// <param name="userId">用户 ID（用于过滤预算等用户级数据）</param>
    /// <param name="periodStart">统计周期开始日期</param>
    /// <param name="periodEnd">统计周期结束日期</param>
    /// <returns>财务画像数据</returns>
    public static Task<FinancialPortraitData> BuildAsync(
        FinanceServiceDbContext dbContext,
        long accountBookId,
        long userId,
        DateTime periodStart,
        DateTime periodEnd)
    {
        return BuildCoreAsync(dbContext, new List<long> { accountBookId }, userId, periodStart, periodEnd,
            isAggregate: false);
    }

    /// <summary>
    /// 构建用户全部账本汇总的财务画像
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    /// <param name="userId">用户 ID</param>
    /// <param name="periodStart">统计周期开始日期</param>
    /// <param name="periodEnd">统计周期结束日期</param>
    /// <returns>财务画像数据，用户无账本时返回仅含基础信息的 portrait</returns>
    public static async Task<FinancialPortraitData> BuildForAllAccountBooksAsync(
        FinanceServiceDbContext dbContext,
        long userId,
        DateTime periodStart,
        DateTime periodEnd)
    {
        var accountBookIds = await dbContext.AccountBooks
            .Where(ab => ab.CreateUserId == userId && !ab.IsDeleted)
            .Select(ab => ab.Id)
            .ToListAsync();

        if (accountBookIds.Count == 0)
        {
            return new FinancialPortraitData
            {
                AccountBookName = "暂无账本",
                AccountBookCount = 0,
                IsAggregate = true,
                PeriodStart = periodStart,
                PeriodEnd = periodEnd
            };
        }

        return await BuildCoreAsync(dbContext, accountBookIds, userId, periodStart, periodEnd,
            isAggregate: accountBookIds.Count > 1);
    }

    // ══════ 核心实现 ═══════════════════════════════════════════════════════════

    /// <summary>
    /// 核心构建方法：加载数据并构建财务画像对象（不涉及数据库持久化）
     /// 支持单账本和全账本两种模式，通过 isAggregate 参数区分
     /// 该方法内部会根据账本数量和 isAggregate 参数智能设置画像中的数据范围说明字段，帮助 LLM 理解画像的适用范围
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="accountBookIds"></param>
    /// <param name="userId"></param>
    /// <param name="periodStart"></param>
    /// <param name="periodEnd"></param>
    /// <param name="isAggregate"></param>
    /// <returns></returns>
    private static async Task<FinancialPortraitData> BuildCoreAsync(
        FinanceServiceDbContext dbContext,
        List<long> accountBookIds,
        long userId,
        DateTime periodStart,
        DateTime periodEnd,
        bool isAggregate)
    {
        // ── 0. 获取账本基础信息与用户账本总数 ──
        int accountBookCount = await dbContext.AccountBooks
            .CountAsync(ab => ab.CreateUserId == userId && !ab.IsDeleted);

        string accountBookName;
        if (isAggregate)
        {
            var names = await dbContext.AccountBooks
                .Where(ab => accountBookIds.Contains(ab.Id))
                .Select(ab => ab.Name)
                .ToListAsync();
            accountBookName = names.Count > 0
                ? $"全部账本（{string.Join("、", names)}）"
                : "全部账本";
        }
        else
        {
            var book = await dbContext.AccountBooks
                .Where(ab => ab.Id == accountBookIds[0] && !ab.IsDeleted)
                .Select(ab => ab.Name)
                .FirstOrDefaultAsync();
            accountBookName = book ?? "未知账本";
        }

        var portrait = new FinancialPortraitData
        {
            AccountBookId = isAggregate ? 0 : accountBookIds[0],
            AccountBookName = accountBookName,
            AccountBookCount = accountBookCount,
            IsAggregate = isAggregate,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd
        };

        // ── 1. 加载当月记账条目 ──
        var periodEntries = await LoadEntriesAsync(dbContext, accountBookIds, periodStart, periodEnd);

        // ── 2. 基础收支数据 ──
        portrait.MonthlyIncome = periodEntries
            .Where(e => e.Type == TransactionCategoryEnmu.Income)
            .Sum(e => e.Amount);
        portrait.MonthlyExpense = periodEntries
            .Where(e => e.Type == TransactionCategoryEnmu.Expenditure)
            .Sum(e => e.Amount);

        // ── 3. 近三个月月均收支 ──
        var threeMonthStart = periodStart.AddMonths(-2);
        var threeMonthEntries = await LoadEntriesAsync(dbContext, accountBookIds, threeMonthStart, periodEnd);
        var monthlyIncomeGroups = threeMonthEntries
            .Where(e => e.Type == TransactionCategoryEnmu.Income)
            .GroupBy(e => new { e.Year, e.Month })
            .Select(g => g.Sum(e => e.Amount))
            .ToList();
        var monthlyExpenseGroups = threeMonthEntries
            .Where(e => e.Type == TransactionCategoryEnmu.Expenditure)
            .GroupBy(e => new { e.Year, e.Month })
            .Select(g => g.Sum(e => e.Amount))
            .ToList();

        portrait.AvgMonthlyIncome3M = monthlyIncomeGroups.Count > 0
            ? Math.Round(monthlyIncomeGroups.Average(), 2)
            : 0;
        portrait.AvgMonthlyExpense3M = monthlyExpenseGroups.Count > 0
            ? Math.Round(monthlyExpenseGroups.Average(), 2)
            : 0;

        // ── 4. 储蓄率趋势（近三个月） ──
        portrait.SavingsTrend3M = threeMonthEntries
            .GroupBy(e => new { e.Year, e.Month })
            .Select(g =>
            {
                decimal inc = g.Where(e => e.Type == TransactionCategoryEnmu.Income).Sum(e => e.Amount);
                decimal exp = g.Where(e => e.Type == TransactionCategoryEnmu.Expenditure).Sum(e => e.Amount);
                return new MonthlySavingsTrend
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    SavingsRate = inc > 0 ? Math.Round((inc - exp) / inc, 4) : 0
                };
            })
            .OrderBy(t => t.Year).ThenBy(t => t.Month)
            .ToList();

        // ── 5. 分类支出明细 ──
        var categoryExpenses = periodEntries
            .Where(e => e.Type == TransactionCategoryEnmu.Expenditure)
            .GroupBy(e => e.CategoryId)
            .ToList();

        if (categoryExpenses.Count > 0 && portrait.MonthlyExpense > 0)
        {
            var categoryIds = categoryExpenses.Select(g => g.Key).ToList();
            var categories = await dbContext.TransactionCategories
                .Where(c => categoryIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c.Name);

            portrait.CategoryDetails = categoryExpenses
                .OrderByDescending(g => g.Sum(e => e.Amount))
                .Select(g =>
                {
                    decimal amount = g.Sum(e => e.Amount);
                    return new CategoryExpenseDetail
                    {
                        CategoryName = categories.TryGetValue(g.Key, out var name) ? name : "未知分类",
                        Amount = amount,
                        Percentage = Math.Round(amount / portrait.MonthlyExpense, 4)
                    };
                })
                .ToList();
        }

        // ── 6. 预算执行情况（预算为用户级数据，按 CreateUserId 过滤） ──
        var budgets = await dbContext.Budgets
            .Where(b => b.CreateUserId == userId
                        && b.StartTime <= periodEnd
                        && b.EndTime >= periodStart
                        && !b.IsDeleted)
            .ToListAsync();

        if (budgets.Count > 0)
        {
            var budgetCategoryIds = budgets.Select(b => b.TransactionCategoryId).Distinct().ToList();
            var budgetCategories = await dbContext.TransactionCategories
                .Where(c => budgetCategoryIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c.Name);

            portrait.BudgetExecution = budgets.Select(b =>
            {
                decimal actual = periodEntries
                    .Where(e => e.CategoryId == b.TransactionCategoryId
                                && e.Type == TransactionCategoryEnmu.Expenditure)
                    .Sum(e => e.Amount);
                decimal overrunRate = b.Amount > 0
                    ? Math.Round((actual - b.Amount) / b.Amount, 4)
                    : 0;
                return new BudgetExecutionDetail
                {
                    CategoryName = budgetCategories.TryGetValue(b.TransactionCategoryId, out var name) ? name : "未知分类",
                    BudgetAmount = b.Amount,
                    ActualAmount = actual,
                    OverrunRate = overrunRate
                };
            }).ToList();
        }

        // ── 7. 收入稳定性数据 ──
        portrait.MonthlyIncomes3M = threeMonthEntries
            .Where(e => e.Type == TransactionCategoryEnmu.Income)
            .GroupBy(e => new { e.Year, e.Month })
            .Select(g => new MonthlyIncomeItem
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Income = g.Sum(e => e.Amount)
            })
            .OrderBy(m => m.Year).ThenBy(m => m.Month)
            .ToList();

        var incomes = portrait.MonthlyIncomes3M.Select(m => m.Income).ToList();
        if (incomes.Count >= 2)
        {
            decimal mean = incomes.Average();
            if (mean > 0)
            {
                decimal variance = incomes.Sum(x => (x - mean) * (x - mean)) / incomes.Count;
                decimal stdDev = (decimal)Math.Sqrt((double)variance);
                portrait.IncomeCV = Math.Round(stdDev / mean, 4);
            }
        }

        // ── 8. 最近一次历史建议 ──
        var lastScore = await dbContext.FinancialHealthScores
            .Where(s => accountBookIds.Contains(s.AccountBookId)
                        && !s.IsDeleted)
            .OrderByDescending(s => s.PeriodEnd)
            .FirstOrDefaultAsync();

        if (lastScore != null)
        {
            portrait.LastTotalScore = lastScore.TotalScore;
            portrait.LastSuggestionContent = lastScore.HealthLevel.ToString();
        }

        return portrait;
    }

    /// <summary>
    /// 从数据库加载指定账本列表、时间段内的记账条目（内部记录）
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="accountBookIds"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    private static async Task<List<EntryRecord>> LoadEntriesAsync(
        FinanceServiceDbContext dbContext,
        List<long> accountBookIds,
        DateTime start,
        DateTime end)
    {
        var raw = await (
            from a in dbContext.Accountings
            join tc in dbContext.TransactionCategories on a.TransactionCategoryId equals tc.Id
            where accountBookIds.Contains(a.AccountBookId)
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
            new EntryRecord(r.AfterAmount, r.Type, r.TransactionCategoryId, r.Year, r.Month)
        ).ToList();
    }

    /// <summary>
    /// 记账条目内部记录
    /// </summary>
    /// <param name="Amount"></param>
    /// <param name="Type"></param>
    /// <param name="CategoryId"></param>
    /// <param name="Year"></param>
    /// <param name="Month"></param>
    private record EntryRecord(
        decimal Amount,
        TransactionCategoryEnmu Type,
        long CategoryId,
        int Year,
        int Month);
}
