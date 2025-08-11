namespace SP.FinanceService.Models.Response;

/// <summary>
/// 用户配置响应模型
/// </summary>
public class ConfigResponse
{
    /// <summary>
    /// 配置id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 配置值
    /// </summary>
    public string Value { get; set; }
}