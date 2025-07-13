using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Response;

/// <summary>
/// 预算响应模型
/// </summary>
public class BudgetResponse
{
    /// <summary>
    /// 预算ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 收支类型id
    /// </summary>
    public long TransactionCategoryId { get; set; }
    
    /// <summary>
    /// 收支类型名称
    /// </summary>
    public string TransactionCategoryName { get; set; }
    
    /// <summary>
    /// 预算金额
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 预算周期
    /// </summary>
    public PeriodEnum Period { get; set; }
    
    /// <summary>
    /// 剩余预算
    /// </summary>
    public decimal Remaining { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime EndTime { get; set; }
}