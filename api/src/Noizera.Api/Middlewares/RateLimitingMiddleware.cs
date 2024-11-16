using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Api.Middlewares;

internal sealed class RateLimitingMiddleware(RequestDelegate next, IMemoryCache cache)
{
    private const int MAX_REQUESTS = 60;
    private const int WINDOW_IN_MINUTES = 1;

    public async Task InvokeAsync([NotNull] HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        if (!string.IsNullOrEmpty(ipAddress) && !context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            var requestCount = cache.GetOrCreate(ipAddress, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(WINDOW_IN_MINUTES);
                return 0;
            });

            if (requestCount >= MAX_REQUESTS)
            {
                throw new Exception($"Too many requests. Please, wait for {WINDOW_IN_MINUTES} min");
            }

            cache.Set(ipAddress, requestCount + 1);
        }

        await next(context).ConfigureAwait(false);
    }
}
