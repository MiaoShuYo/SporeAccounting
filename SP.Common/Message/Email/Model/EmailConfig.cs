namespace SP.Common.Message.Email.Model;

/// <summary>
/// 邮件配置类
/// </summary>
public class EmailConfig
{
    /// <summary>
    /// 邮件发送者
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// smtp服务器
    /// </summary>
    public string SmtpServer { get; set; }

    /// <summary>
    /// smtp端口
    /// </summary>
    public int SmtpPort { get; set; }

    /// <summary>
    /// smtp 用户
    /// </summary>
    public string SmtpUser { get; set; }

    /// <summary>
    /// smtp 密码
    /// </summary>
    public string SmtpPassword { get; set; }
}