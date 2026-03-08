using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Request;

/// <summary>
/// 账本分享请求
/// </summary>
public class AccountBookShareAddRequest
{
    /// <summary>
    /// 账本id
    /// </summary>
    public long AccountBookId { get; set; }

    /// <summary>
    /// 用户id
    /// </summary>
    public List<long> UserId { get; set; }

    /// <summary>
    /// 权限类型（0-只读，1-读写，2-管理）
    /// </summary>
    public PermissionTypeEnum PermissionType { get; set; }
}