namespace SP.FinanceService.Mq.Models;

/// <summary>
/// 预算变更消息模型
/// </summary>
public class BudgetChangeMQ
{
    /// <summary>
    /// 变更金额
    /// </summary>
    public decimal ChangeAmount { get; set; }
    
    /// <summary>
    /// 收支分类Id
    /// </summary>
    public long TransactionCategoryId { get; set; }
    
    /// <summary>
    /// 用户id
    /// </summary>
    public long UserId { get; set; }
}