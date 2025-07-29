namespace SP.FinanceService.Models.Response;

/// <summary>
/// 汇率记录响应
/// </summary>
public class ExchangeRateRecordResponse
{
    /// <summary>
    /// 汇率记录id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 汇率
    /// </summary>
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// 币种转换
    /// </summary>
    public string ConvertCurrency { get; set; }

    /// <summary>
    /// 汇率日期
    /// </summary>
    public DateTime Date { get; set; }
} 