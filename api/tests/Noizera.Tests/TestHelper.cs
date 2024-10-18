using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace Noizera.Tests;

internal static class TestHelper
{
    public static StringContent ToJsonRequest<T>(T request)
    {
        var jsonContent = JsonSerializer.Serialize(request);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        return content;
    }

    public static async Task<ProblemDetails?> ToProblemDetails(HttpResponseMessage response) => await To<ProblemDetails>(response);

    public static async Task<T?> To<T>(HttpResponseMessage response)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var message = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<T>(message, options);

        return result;
    }
}
