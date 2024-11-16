using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Noizera.Common.Infrastructure.Emails;

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
        using MimeMessage message = new();
        message.From.Add(new MailboxAddress(nameFrom, emailFrom));
        message.To.Add(new MailboxAddress(nameTo, emailTo));
        message.Subject = subject;

        BodyBuilder bodyBuilder = new()
        {
            HtmlBody = content
        };
        message.Body = bodyBuilder.ToMessageBody();

        using SmtpClient client = new();
        await client.ConnectAsync(settings.Server, settings.Port, SecureSocketOptions.StartTls).ConfigureAwait(false);
        await client.AuthenticateAsync(settings.Username, settings.Password).ConfigureAwait(false);
        _ = await client.SendAsync(message).ConfigureAwait(false);
        await client.DisconnectAsync(true).ConfigureAwait(false);
    }
}
