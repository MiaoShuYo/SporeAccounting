using Microsoft.Extensions.Configuration;
using SP.Common.Message.Email.Model;

namespace SP.Common.ConfigService;

/// <summary>
/// 邮件配置服务
/// </summary>
public class EmailConfigService
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// 邮件配置服务构造函数
    /// </summary>
    /// <param name="configuration"></param>
    public EmailConfigService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public EmailConfig GetEmailConfig()
    {
        var emailConfig = new EmailConfig();
        _configuration.GetSection("EmailConfig").Bind(emailConfig);
        return emailConfig;
    }
}