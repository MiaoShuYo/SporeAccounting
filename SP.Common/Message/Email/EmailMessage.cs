using System.Net;
using System.Net.Mail;
using SP.Common.Message.Email.Model;

namespace SP.Common.Message.Email;

/// <summary>
/// 邮件发送类
/// </summary>
public class EmailMessage
{
    private readonly EmailConfig _emailConfig;

    /// <summary>
    /// 构造函数，注入EmailConfig
    /// </summary>
    /// <param name="emailConfig"></param>
    public EmailMessage(EmailConfig emailConfig)
    {
        _emailConfig = emailConfig;
    }

    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="subject">主题</param>
    /// <param name="content">内容</param>
    /// <returns></returns>
    public async Task<bool> SendEmailAsync(string email, string subject, string content)
    {
        try
        {
            using var smtpClient = new SmtpClient(_emailConfig.SmtpServer, _emailConfig.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailConfig.SmtpUser, _emailConfig.SmtpPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailConfig.Email),
                Subject = subject,
                Body = content,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}