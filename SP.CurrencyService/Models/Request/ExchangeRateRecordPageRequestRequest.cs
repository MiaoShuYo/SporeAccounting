using System.ComponentModel.DataAnnotations;
using SP.Common.Model;

namespace SP.CurrencyService.Models.Request;

/// <summary>
/// 汇率分页查询请求
/// </summary>
public class ExchangeRateRecordPageRequestRequest:PageRequestModel
{
    /// <summary>
    /// 源货币
    /// </summary>
    [Required(ErrorMessage = "源货币不能为空")]
    public long SourceCurrencyId { get; set; }
    /// <summary>
    /// 目标货币
    /// </summary>
    [Required(ErrorMessage = "目标货币不能为空")]
    public long TargetCurrencyId { get; set; }
}