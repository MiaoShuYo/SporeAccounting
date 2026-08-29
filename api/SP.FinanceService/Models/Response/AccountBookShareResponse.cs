using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Response;

/// <summary>
/// 账本分享响应模型
/// </summary>
public class AccountBookShareResponse
{
    /// <summary>
    /// 分享id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 账本id
    /// </summary>
    public long AccountBookId { get; set; }

    /// <summary>
    /// 用户id
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 权限类型
    /// </summary>
    public PermissionTypeEnum PermissionType { get; set; }
}