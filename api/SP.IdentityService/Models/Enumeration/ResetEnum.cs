namespace SP.IdentityService.Models.Enumeration;

/// <summary>
/// 密码找回方式
/// </summary>
public enum ResetEnum
{
    /// <summary>
    /// 邮箱找回
    /// </summary>
    Email = 1,

    /// <summary>
    /// 手机号找回
    /// </summary>
    Phone = 2,
}