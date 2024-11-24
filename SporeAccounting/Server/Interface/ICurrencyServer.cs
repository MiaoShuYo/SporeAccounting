using SporeAccounting.Models;

namespace SporeAccounting.Server.Interface;

/// <summary>
/// 币种服务接口
/// </summary>
public interface ICurrencyServer
{
    /// <summary>
    /// 查询全部币种
    /// </summary>
    /// <returns></returns>
    IQueryable<Currency> Query();
    /// <summary>
    /// 查询币种
    /// </summary>
    /// <param name="currencyId"></param>
    /// <returns></returns>
    Currency? Query(string currencyId);
}