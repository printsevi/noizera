using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Noizera.Shared.Infrastructure.Emails;

public class EmailSender(IOptions<EmailSettings> emailSettings)
{
    private readonly EmailSettings settings = emailSettings.Value;

    public async Task SendAsync(
        string emailFrom,
        string nameFrom,
        string emailTo,
        string nameTo,
        string subject,
        string content)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(nameFrom, emailFrom));
        message.To.Add(new MailboxAddress(nameTo, emailTo));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = content
        };
        message.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(settings.Server, settings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(settings.Username, settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
