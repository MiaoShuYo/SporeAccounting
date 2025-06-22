using SP.Common.Model;

namespace SP.CurrencyService.Models.Request;

/// <summary>
/// 汇率分页查询请求
/// </summary>
public class ExchangeRateRecordPageRequest:PageModel
{
    /// <summary>
    /// 币种
    /// </summary>
    public string Currency { get; set; }
}