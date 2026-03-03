namespace SP.FinanceService.Models.Enumeration;

/// <summary>
/// 分摊状态
/// </summary>
public enum SharedExpenseStatusEnum
{
    /// <summary>
    /// 未付款
    /// </summary>
    Unpaid=0,

    /// <summary>
    /// 部分付款
    /// </summary>
    PartialPayment=1,

    /// <summary>
    /// 全部分款
    /// </summary>
    All =2
}