namespace SporeAccounting.Models.ViewModels;

/// <summary>
/// 用户配置视图模型
/// </summary>
public class ConfigViewModel
{
    /// <summary>
    /// 配置id
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// 配置值
    /// </summary>
    public string Value { get; set; }
    /// <summary>
    /// 配置类型
    /// </summary>
    public ConfigTypeEnum ConfigTypeEnum { get; set; }
}