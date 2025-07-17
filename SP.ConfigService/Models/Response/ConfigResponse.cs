using SP.Config.Models.Enumeration;

namespace SP.Config.Models.Response;

/// <summary>
/// 用户配置响应模型
/// </summary>
public class ConfigResponse
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
    /// 配置值的名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 配置类型
    /// </summary>
    public ConfigTypeEnum ConfigType { get; set; }
}