using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Noizera.Shared.Persistence.Mongo;

namespace Noizera.Shared.Infrastructure.Emails;

public class EmailService(
    EmailSender sender,
    MongoDbContext db)
{
    public async Task UploadEmailTemplate(string emailTemplateType, IFormFile htmlFile)
    {
        if (htmlFile == null || htmlFile.Length == 0)
        {
            throw new Exception("File is empty.");
        }

        using (var streamReader = new StreamReader(htmlFile.OpenReadStream()))
        {
            var htmlContent = await streamReader.ReadToEndAsync();

            await db.UpsertEmailTemplateAsync(emailTemplateType, htmlContent);
        }
    }

    public async Task SendEmailAsync(
        string templateName,
        string emailTo,
        string nameTo,
        string emailFrom,
        string nameFrom,
        string subject,
        IDictionary<string, string>? idValuePairs = null)
    {
        var htmlContent = await db.GetEmailTemplateContent(templateName);
        if (string.IsNullOrWhiteSpace(htmlContent))
        {
            throw new Exception("Template not found.");
        }

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        if (idValuePairs is not null)
        {
            foreach (var pair in idValuePairs)
            {
                var div = htmlDoc.DocumentNode.SelectSingleNode($"//div[@id='{pair.Key}']");
                if (div is not null)
                {
                    div.InnerHtml = pair.Value;
                }
            }
        }

        await sender.SendAsync(emailFrom, nameFrom, emailTo, nameTo, subject, htmlDoc.DocumentNode.OuterHtml);
    }
}
