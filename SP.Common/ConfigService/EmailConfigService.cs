using Microsoft.Extensions.Configuration;
using SP.Common.Message.Model;

namespace SP.Common;

public class EmailConfigService
{
    private readonly IConfiguration _configuration;
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