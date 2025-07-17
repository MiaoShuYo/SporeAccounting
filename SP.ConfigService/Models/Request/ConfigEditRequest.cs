using System.ComponentModel.DataAnnotations;
using SP.ConfigService.Models.Enumeration;

namespace SP.ConfigService.Models.Request;

/// <summary>
/// 用户配置修改请求模型
/// </summary>
public class ConfigEditRequest
{
    /// <summary>
    /// 配置id
    /// </summary>
    [Required(ErrorMessage = "配置id不能为空")]
    public long Id { get; set; }

    /// <summary>
    /// 配置类型
    /// </summary>
    [Required(ErrorMessage = "配置类型不能为空")]
    public ConfigTypeEnum ConfigType { get; set; }

    /// <summary>
    /// 配置的值
    /// </summary>
    [Required(ErrorMessage = "配置值不能为空")]
    [MaxLength(100, ErrorMessage = "配置值长度不能超过100")]
    public string Value { get; set; }
}