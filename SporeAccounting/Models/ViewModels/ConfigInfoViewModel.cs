﻿namespace SporeAccounting.Models.ViewModels;

/// <summary>
/// 配置信息视图模型
/// </summary>
public class ConfigInfoViewModel
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