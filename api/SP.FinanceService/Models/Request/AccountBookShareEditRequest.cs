using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Request;

/// <summary>
/// 账本分享编辑请求
/// </summary>
public class AccountBookShareEditRequest
{
    /// <summary>
    /// 账本id
    /// </summary>
    public long AccountBookId { get; set; }

    /// <summary>
    /// 用户id集合
    /// </summary>
    public List<long> UserIds { get; set; }

    /// <summary>
    /// 权限 类型（0-只读，1-读写，2-管理）
    /// </summary>
    public PermissionTypeEnum PermissionType { get; set; }
}