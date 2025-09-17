using SP.ResourceService.Models.Response;

namespace SP.ResourceService.Service;

/// <summary>
/// AI助手服务
/// </summary>
public interface IAssistantService
{
    /// <summary>
    /// 提取文字中的金额和消费类型
    /// </summary>
    /// <param name="text">文字内容</param>
    /// <returns>金额和消费类型</returns>
    Task<AmountAndCategoryExtractionResponse> ExtractAmountAndCategoryAsync(string text);
}