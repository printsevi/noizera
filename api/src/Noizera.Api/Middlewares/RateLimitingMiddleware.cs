using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Api.Middlewares;

internal sealed class RateLimitingMiddleware(RequestDelegate next, IMemoryCache cache)
{
    private const int maxRequests = 60;
    private const int windowInMinutes = 1;

    public async Task InvokeAsync([NotNull] HttpContext context)
    {
        string? ipAddress = context.Connection.RemoteIpAddress?.ToString();
        if (!string.IsNullOrEmpty(ipAddress) && !context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            int requestCount = await cache.GetOrCreateAsync(ipAddress, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(windowInMinutes);
                return Task.FromResult(0);
            }).ConfigureAwait(false);

            if (requestCount >= maxRequests)
            {
                throw new AuthenticationFailureException($"Too many requests. Please, wait for {windowInMinutes} min");
            }

            _ = cache.Set(ipAddress, requestCount + 1);
        }

        await next(context).ConfigureAwait(false);
    }
}
