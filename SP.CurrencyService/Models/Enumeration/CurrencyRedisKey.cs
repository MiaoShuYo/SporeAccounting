namespace SP.CurrencyService.Models.Enumeration;

/// <summary>
/// 币种Redis键枚举
/// </summary>
public class CurrencyRedisKey
{
    /// <summary>
    /// 币种列表的Redis键
    /// </summary>
    public const string Currency = "Currency";

    /// <summary>
    /// 汇率列表的Redis键
    /// </summary>
    public const string ExchangeRate = "ExchangeRate:{0}:{1}:{2}";
}