namespace SP.FinanceService.Models.Response;

public class BudgetGenerationResponse
{
    /// <summary>
    /// AI生成预算id
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
    public int Period { get; set; }
    
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 预算开始时间
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 预算结束时间
    /// </summary>
    public DateTime EndTime { get; set; }
}