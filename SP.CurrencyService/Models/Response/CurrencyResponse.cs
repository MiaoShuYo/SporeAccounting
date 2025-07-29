namespace SP.CurrencyService.Models.Response;

/// <summary>
/// 币种响应
/// </summary>
public class CurrencyResponse
{
    /// <summary>
    /// 币种id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 币种名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 币种缩写
    /// </summary>
    public string Abbreviation { get; set; }
}