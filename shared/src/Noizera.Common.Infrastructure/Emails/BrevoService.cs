using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;

namespace Noizera.Common.Infrastructure.Emails;

public class BrevoService
{
    private readonly HttpClient httpClient;
    private readonly string apiKey;

    public BrevoService(HttpClient httpClient, [NotNull] IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(configuration["BrevoApiKey"]);

        this.httpClient = httpClient;
        apiKey = configuration["BrevoApiKey"]!;
        this.httpClient.DefaultRequestHeaders.Add("api-key", apiKey);
        this.httpClient.BaseAddress = new Uri("https://api.brevo.com/v3/");
    }

    public async Task SendLoginConfirmationAsync(string recipientEmail, string recipientName, string code, CancellationToken ct)
        => await SendTransactionalEmailAsync(
            recipientEmail,
            recipientName,
            3,
            ct,
            new Dictionary<string, object>() { { "code", code } }
        ).ConfigureAwait(false);

    public async Task SendContactFormAsync(string recipientEmail, string recipientName, string topic, string description, CancellationToken ct)
        => await SendTransactionalEmailAsync(
            recipientEmail,
            recipientName,
            5,
            ct,
            new Dictionary<string, object>() { { "description", description } },
            [("help@noizera.com", "Noizera team")],
            $"{topic} from {recipientEmail}"
        ).ConfigureAwait(false);

    private async Task SendTransactionalEmailAsync(string recipientEmail, string recipientName, long templateId, CancellationToken ct, Dictionary<string, object>? parameters = null, List<(string Email, string Name)>? cc = null, string? subject = null)
    {
        Dictionary<string, object> baseParameters = new()
        {
            { "name", recipientName }
        };

        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                baseParameters[param.Key] = param.Value;
            }
        }

        var emailPayload = new
        {
            to = new[]
            {
                new { email = recipientEmail, name = recipientName }
            },
            cc = cc?.Select(c => new { email = c.Email, name = c.Name }).ToArray(),
            subject,
            templateId,
            @params = baseParameters
        };

        var response = await httpClient.PostAsJsonAsync("smtp/email", emailPayload, ct).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new InvalidOperationException($"Failed to send email: {error}");
        }
    }
}
