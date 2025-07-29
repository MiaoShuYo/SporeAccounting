namespace SP.IdentityService.Models.Request;

public class UserPageRequest : PageRequest
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