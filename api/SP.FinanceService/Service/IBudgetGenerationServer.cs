using SP.FinanceService.Models.Request;
using SP.FinanceService.Models.Response;

namespace SP.FinanceService.Service;

/// <summary>
/// 预算生成服务接口
/// </summary>
public interface IBudgetGenerationServer
{
    /// <summary>
    /// AI生成预算，返回预览数据
    /// </summary>
    /// <param name="request">生成请求</param>
    /// <returns>预览数据</returns>
    Task<List<BudgetGenerationResponse>> GenerateBudget(BudgetGenerationRequest request);

    /// <summary>
    /// 确认/取消生成预算，返回生成结果
    /// </summary>
    /// <param name="id">AI生成预算id</param>
    /// <param name="confirm">是否确认生成</param>
    /// <returns>任务</returns>
    System.Threading.Tasks.Task ConfirmBudget(long id, bool confirm);
}