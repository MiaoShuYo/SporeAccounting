using System.Text.Json.Serialization;

namespace SporeAccounting.Task.Timer.Model;

/// <summary>
/// 汇率同步接口类
/// </summary>
public class ExchangeRateApiData
{
    /// <summary>
    /// 数据状态
    /// </summary>
    [JsonPropertyName("result")]
    public string Result { get; set; }

    /// <summary>
    /// 数据文档
    /// </summary>
    [JsonPropertyName("documentation")]
    public string Documentation { get; set; }

    /// <summary>
    /// 数据条款
    /// </summary>
    [JsonPropertyName("terms_of_use")]
    public string TermsOfUse { get; set; }

    /// <summary>
    /// 上次更新数据时间戳
    /// </summary>
    [JsonPropertyName("time_last_update_unix")]
    public long TimeLastUpdateUnix { get; set; }

    /// <summary>
    /// 上次更新数据时间
    /// </summary>
    [JsonPropertyName("time_last_update_utc")]
    public string TimeLastUpdateUtc { get; set; }

    /// <summary>
    /// 下次更新数据时间戳
    /// </summary>
    [JsonPropertyName("time_next_update_unix")]
    public long TimeNextUpdateUnix { get; set; }

    /// <summary>
    /// 下次更新数据时间
    /// </summary>
    [JsonPropertyName("time_next_update_utc")]
    public string TimeNextUpdateUtc { get; set; }

    /// <summary>
    /// 基础货币代码
    /// </summary>
    [JsonPropertyName("base_code")]
    public string BaseCode { get; set; }

    /// <summary>
    /// 汇率集合
    /// </summary>
    [JsonPropertyName("conversion_rates")]
    public Dictionary<string, decimal> ConversionRates { get; set; }
}