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

    public async Task SendTransactionalEmailAsync(string recipientEmail, string recipientName, long templateId, Dictionary<string, object>? parameters = null)
    {
        var emailPayload = new
        {
            to = new[]
            {
                new { email = recipientEmail, name = recipientName }
            },
            templateId,
            @params = parameters ?? []
        };

        var response = await httpClient.PostAsJsonAsync("smtp/email", emailPayload).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new InvalidOperationException($"Failed to send email: {error}");
        }
    }
}
