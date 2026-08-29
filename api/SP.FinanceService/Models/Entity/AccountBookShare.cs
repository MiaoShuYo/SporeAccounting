using System.ComponentModel.DataAnnotations.Schema;
using SP.Common.Model;
using SP.FinanceService.Models.Enumeration;

namespace SP.FinanceService.Models.Entity;

/// <summary>
/// 账本分享
/// </summary>
[Table("AccountBookShare")]
public class AccountBookShare : BaseModel
{
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