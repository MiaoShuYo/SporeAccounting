using SP.Common.ExceptionHandling.Exceptions;
using SP.FinanceService.Models.Response;
using SP.FinanceService.RefitClient;

namespace SP.FinanceService.Service.Impl;

/// <summary>
/// 货币服务实现类
/// </summary>
public class CurrencyServiceImpl : ICurrencyService
{
    private readonly ICurrencyServiceApi _currencyServiceApi;
    private readonly ILogger<CurrencyServiceImpl> _logger;

    public CurrencyServiceImpl(ICurrencyServiceApi currencyServiceApi, ILogger<CurrencyServiceImpl> logger)
    {
        _currencyServiceApi = currencyServiceApi;
        _logger = logger;
    }

    /// <summary>
    /// 获取两个币种之间的今日汇率
    /// </summary>
    /// <param name="sourceCurrencyId">源币种ID</param>
    /// <param name="targetCurrencyId">目标币种ID</param>
    /// <returns>返回今日汇率记录</returns>
    public async Task<ExchangeRateRecordResponse> GetTodayExchangeRateByCode(long sourceCurrencyId, long targetCurrencyId)
    {
        try
        {
            // 调用货币服务API获取今日汇率
            var response = await _currencyServiceApi.GetTodayExchangeRateByCode(sourceCurrencyId, targetCurrencyId);
            
            // 检查响应是否成功，并且内容不为空
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                return response.Content;
            }
            
            _logger.LogError("获取汇率失败: {StatusCode}, {ErrorMessage}", 
                response.StatusCode, response.Error?.Content);
            
            throw new RefitException($"获取汇率失败: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调用货币服务获取汇率时发生异常");
            throw new RefitException("获取汇率时发生异常", ex);
        }
    }
} 