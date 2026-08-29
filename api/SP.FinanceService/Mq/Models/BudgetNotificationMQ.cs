namespace SP.FinanceService.Mq.Models;

/// <summary>
/// 预算通知消息模型
/// </summary>
public class BudgetNotificationMQ
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }
    
    /// <summary>
    /// 预算ID
    /// </summary>
    public long BudgetId { get; set; }
    
    /// <summary>
    /// 使用百分比（预警时使用）
    /// </summary>
    public decimal? UsagePercent { get; set; }
    
    /// <summary>
    /// 超额百分比（超额时使用）
    /// </summary>
    public decimal? OverrunPercent { get; set; }
    
    /// <summary>
    /// 通知方式偏好（来自用户配置）
    /// </summary>
    public int MessagePreference { get; set; }
}

