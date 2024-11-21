using SporeAccounting.Models;

namespace SporeAccounting.Server.Interface;

/// <summary>
/// 币种服务接口
/// </summary>
public interface ICurrencyService
{
    /// <summary>
    /// 查询全部币种
    /// </summary>
    /// <returns></returns>
    IQueryable<Currency> Query();
}