namespace SP.FinanceService.Models.Request;

/// <summary>
/// 取消共享账本请求
/// </summary>
public class AccountBookRevokeSharingRequest
{
    /// <summary>
    /// 账本ID
    /// </summary>
    public long AccountBookId { get; set; }

    /// <summary>
    /// 被取消共享的用户ID集合
    /// </summary>
    public List<long> UserIds { get; set; }
}