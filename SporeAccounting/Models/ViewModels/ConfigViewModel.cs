using System.ComponentModel.DataAnnotations;

namespace SporeAccounting.Models.ViewModels;

/// <summary>
/// 用户配置视图模型
/// </summary>
public class ConfigViewModel
{
    /// <summary>
    /// 配置id
    /// </summary>
    [Required(ErrorMessage = "配置id不能为空")]
    public string Id { get; set; }

    /// <summary>
    /// 配置值
    /// </summary>
    [Required(ErrorMessage = "配置值不能为空")]
    [MaxLength(100, ErrorMessage = "配置值最大长度为100")]
    public string Value { get; set; }

    /// <summary>
    /// 配置类型
    /// </summary>
    [Required(ErrorMessage = "配置类型不能为空")]
    public ConfigTypeEnum ConfigTypeEnum { get; set; }
}