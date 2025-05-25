namespace SP.IdentityService.Models.Request;

/// <summary>
/// 用户修改请求
/// </summary>
public class UserUpdateRequest
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 电子邮件
    /// </summary>
    public string Email { get; set; } = string.Empty;
}