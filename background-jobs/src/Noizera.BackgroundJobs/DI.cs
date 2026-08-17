using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Noizera.BackgroundJobs.Common;
using Noizera.BackgroundJobs.Jobs;
using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Outbox;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Noizera.BackgroundJobs;

internal static class DI
{
    /// <summary>How stale the oldest unprocessed message may get before the job is unhealthy.</summary>
    private static readonly TimeSpan maxOutboxAge = TimeSpan.FromMinutes(15);

    public static IServiceCollection AddServices(this IServiceCollection services, [NotNull] IConfiguration configuration)
    {
        _ = services.Configure<EventSettings>(configuration.GetSection("EventSettings"));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddSingleton<RealTimeOutboxBackgroundJob>();
        services.AddHostedService(
            provider => provider.GetRequiredService<RealTimeOutboxBackgroundJob>());

        services.AddSingleton<DelayedOutboxBackgroundJob>();
        services.AddHostedService(
            provider => provider.GetRequiredService<DelayedOutboxBackgroundJob>());

        return services;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app)
    {
        app.MapPatch("/real-time-job", ([FromServices] RealTimeOutboxBackgroundJob job, bool enabled) =>
        {
            job.IsEnabled = enabled;
        });

        app.MapPatch("/delayed-job", ([FromServices] DelayedOutboxBackgroundJob job, bool enabled) =>
        {
            job.IsEnabled = enabled;
        });

        // Outbox age is the single most informative signal in this service: it goes non-zero
        // for a dead pump, a poison message, an unreachable Stripe/Brevo, or a failing ffmpeg.
        app.MapGet("/health", async (
            [FromServices] RealTimeOutboxBackgroundJob realTimeJob,
            [FromServices] DelayedOutboxBackgroundJob delayedJob,
            [FromServices] AppDbContext db,
            CancellationToken ct) =>
        {
            var now = SystemClock.UtcNow;

            DateTimeOffset? oldestPending = await db.OutboxMessages
                .Where(x => x.ProcessedOn == null && x.ProcessAfter <= now)
                .OrderBy(x => x.OccurredOn)
                .Select(x => (DateTimeOffset?)x.OccurredOn)
                .FirstOrDefaultAsync(ct).ConfigureAwait(false);

            var outboxAge = oldestPending.HasValue ? now - oldestPending.Value : TimeSpan.Zero;

            var jobs = new[]
            {
                Describe(nameof(RealTimeOutboxBackgroundJob), realTimeJob.IsEnabled, realTimeJob.IsStopped, realTimeJob.ConsecutiveFailures, realTimeJob.LastSuccessfulRunAt),
                Describe(nameof(DelayedOutboxBackgroundJob), delayedJob.IsEnabled, delayedJob.IsStopped, delayedJob.ConsecutiveFailures, delayedJob.LastSuccessfulRunAt),
            };

            bool healthy = !realTimeJob.IsStopped
                && !delayedJob.IsStopped
                && outboxAge < maxOutboxAge;

            var payload = new
            {
                status = healthy ? "Healthy" : "Unhealthy",
                outboxAgeSeconds = (long)outboxAge.TotalSeconds,
                maxOutboxAgeSeconds = (long)maxOutboxAge.TotalSeconds,
                jobs,
            };

            return healthy ? Results.Ok(payload) : Results.Json(payload, statusCode: StatusCodes.Status503ServiceUnavailable);
        });

        return app;
    }

    private static object Describe(string name, bool isEnabled, bool isStopped, int consecutiveFailures, DateTimeOffset? lastSuccessfulRunAt)
        => new
        {
            name,
            isEnabled,
            isStopped,
            consecutiveFailures,
            lastSuccessfulRunAt,
        };
}
