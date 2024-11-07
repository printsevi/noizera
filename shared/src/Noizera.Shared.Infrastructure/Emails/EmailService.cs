using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Noizera.Shared.Persistence.S3;

namespace Noizera.Shared.Infrastructure.Emails;

public class EmailService(EmailSender sender, S3Context s3)
{
    public async Task UploadEmailTemplate(string emailTemplateType, IFormFile htmlFile, CancellationToken ct)
    {
        if (htmlFile == null || htmlFile.Length == 0)
        {
            throw new Exception($"File {emailTemplateType} is empty");
        }

        await s3.UploadEmailTemplateAsync(emailTemplateType, htmlFile, ct).ConfigureAwait(false);
    }

    public async Task SendEmailAsync(
        string templateName,
        string emailTo,
        string nameTo,
        string emailFrom,
        string nameFrom,
        string subject,
        CancellationToken ct,
        IDictionary<string, string>? idValuePairs = null)
    {
        var htmlContent = await s3.GetEmailTemplateContentAsync(templateName, ct);
        if (string.IsNullOrWhiteSpace(htmlContent))
        {
            throw new Exception($"Template not found: {templateName}");
        }

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        if (idValuePairs is not null)
        {
            foreach (var pair in idValuePairs)
            {
                var div = htmlDoc.DocumentNode.SelectSingleNode(pair.Key);
                if (div is not null)
                {
                    div.InnerHtml = pair.Value;
                }
            }
        }

        await sender.SendAsync(emailFrom, nameFrom, emailTo, nameTo, subject, htmlDoc.DocumentNode.OuterHtml);
    }
}
