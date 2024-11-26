using Docker.DotNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Noizera.Common.Infrastructure.Audio;
using Noizera.Common.Infrastructure.DataStructure;
using Noizera.Common.Infrastructure.Emails;
using Noizera.Common.Infrastructure.Subscriptions;
using Noizera.Common.Persistence;
using Serilog;
using SerilogTracing;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Noizera.Common.Infrastructure;

public static class DI
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, [NotNull] WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        services.AddSharedPersistence(configuration);

        _ = services.Configure<DataDirectoryStructure>(configuration.GetSection("DataDirectoryStructure"));
        _ = services.AddSingleton<DataStructureProvider>();

        StripeConfiguration.ApiKey = configuration["StripeApiKey"];
        _ = services.AddScoped<SessionService>();
        _ = services.AddScoped<CustomerService>();
        _ = services.AddScoped<SubscriptionService>();
        _ = services.AddScoped<Stripe.BillingPortal.SessionService>();
        _ = services.AddScoped<StripeService>();
        _ = services.AddScoped<SubscriptionStripeService>();

        _ = services.AddTransient<IDockerClient>(_ => new DockerClientConfiguration(new Uri(configuration["DockerUrl"]!)).CreateClient());

        _ = services.AddScoped<FfmpegDockerService>();
        _ = services.AddScoped<AudioService>();
        _ = services.Configure<AudioSettings>(configuration.GetSection("AudioSettings"));

        _ = builder.Services.AddHttpClient<BrevoService>();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .WriteTo.Seq(configuration["SeqUrl"]!, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information, apiKey: configuration["SeqApiKey"]!)
            .Enrich.WithProperty("AppName", configuration["AppName"]!)
            .Enrich.FromLogContext()
            .Enrich.WithSpanTiming()
            .CreateLogger();

        _ = builder.Host.UseSerilog();

        return services;
    }
}
