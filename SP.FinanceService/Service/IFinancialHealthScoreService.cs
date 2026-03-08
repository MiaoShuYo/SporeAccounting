using System.Threading.Tasks;
using SP.Common.Model;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service;

/// <summary>
/// 财务健康评分服务接口
/// </summary>
public interface IFinancialHealthScoreService
{
    /// <summary>
    /// 计算并保存指定账本、指定周期的财务健康评分
    /// </summary>
    /// <param name="accountBookId">账本 ID</param>
    /// <param name="periodStart">统计周期开始日期</param>
    /// <param name="periodEnd">统计周期结束日期</param>
    System.Threading.Tasks.Task<FinancialHealthScoreResponse> CalculateAndSaveAsync(long accountBookId, DateTime periodStart, DateTime periodEnd);

    /// <summary>
    /// 获取指定账本的最新一条评分记录
    /// </summary>
    /// <param name="accountBookId">账本 ID</param>
    FinancialHealthScoreResponse? GetLatestScore(long accountBookId);

    /// <summary>
    /// 分页获取指定账本的历史评分记录
    /// </summary>
    /// <param name="accountBookId">账本 ID</param>
    /// <param name="page">页码（从 1 开始）</param>
    /// <param name="size">每页条数</param>
    PageResponse<FinancialHealthScoreResponse> GetScoreHistory(long accountBookId, int page, int size);

    /// <summary>
    /// 基于当月数据实时生成改善建议（不持久化）
    /// </summary>
    /// <param name="accountBookId">账本 ID</param>
    System.Threading.Tasks.Task<List<FinancialSuggestionResponse>> GetSuggestionsAsync(long accountBookId);

    /// <summary>
    /// 为所有账本计算上月评分（供定时任务调用）
    /// </summary>
    System.Threading.Tasks.Task CalculateMonthlyScoresAsync();
}
