using SP.CurrencyService.Models.Entity;
using SP.CurrencyService.Models.Response;

namespace SP.CurrencyService.Service;

/// <summary>
/// 货币服务
/// </summary>
public interface ICurrencyServer
{
    /// <summary>
    /// 查询所有货币
    /// </summary>
    /// <returns>返回货币列表</returns>
    List<CurrencyResponse> Query();
}