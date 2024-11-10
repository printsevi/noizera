using Microsoft.AspNetCore.Mvc;
using Noizera.BackgroundJobs.Common;
using Noizera.BackgroundJobs.Jobs;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Noizera.BackgroundJobs;

internal static class DI
{
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

        return app;
    }
}
