namespace SP.IdentityService.Models.Response;

/// <summary>
/// 用户响应
/// </summary>
public class UserResponse
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 电子邮件
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 是否锁定
    /// </summary>
    public bool IsLocked { get; set; } = false;
}