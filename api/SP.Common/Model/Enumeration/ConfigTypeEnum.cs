namespace SP.Common.Model.Enumeration;

/// <summary>
/// 用户配置类型
/// </summary>
public enum ConfigTypeEnum
{
    /// <summary>
    /// 币种
    /// </summary>
    Currency = 0,

    /// <summary>
    /// 通知方式
    /// </summary>
    Notification = 1,

    /// <summary>
    /// 预算报警阈值
    /// </summary>
    BudgetAlertThreshold = 2,

    /// <summary>
    /// 提醒频率
    /// </summary>
    ReminderFrequency = 3,

    /// <summary>
    /// 静音时段
    /// </summary>
    SilentPeriod = 4,

    /// <summary>
    /// 使用主题
    /// </summary>
    Theme = 5,

    /// <summary>
    /// 字体设置
    /// </summary>
    Font = 6,

    /// <summary>
    /// 数据展示方式（表格、图表）
    /// </summary>
    Data = 7,
}