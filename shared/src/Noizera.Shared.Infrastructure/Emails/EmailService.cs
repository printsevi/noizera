using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Noizera.Common.Persistence.S3;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Common.Infrastructure.Emails;

public class EmailService([NotNull] EmailSender sender, [NotNull] S3Context s3)
{
    public async Task UploadEmailTemplate(string emailTemplateType, [NotNull] IFormFile htmlFile, [NotNull] CancellationToken ct)
    {
        if (htmlFile == null || htmlFile.Length == 0)
        {
            throw new ArgumentNullException($"File {emailTemplateType} is empty");
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
        [NotNull] CancellationToken ct,
        IDictionary<string, string>? idValuePairs = null)
    {
        string htmlContent = await s3.GetEmailTemplateContentAsync(templateName, ct).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(htmlContent))
        {
            throw new ArgumentNullException($"Template not found: {templateName}");
        }

        HtmlDocument htmlDoc = new();
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

        await sender.SendAsync(emailFrom, nameFrom, emailTo, nameTo, subject, htmlDoc.DocumentNode.OuterHtml).ConfigureAwait(false);
    }
}
