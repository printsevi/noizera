using Docker.DotNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Noizera.Shared.Infrastructure.Audio;
using Noizera.Shared.Infrastructure.DataStructure;
using Noizera.Shared.Infrastructure.Emails;
using Noizera.Shared.Infrastructure.Subscriptions;
using Noizera.Shared.Persistence;
using Serilog;
using SerilogTracing;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Shared.Infrastructure;

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

        _ = services.AddScoped<EmailService>();
        _ = services.AddScoped<EmailSender>();
        _ = services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.Seq(configuration["SeqUrl"]!)
            .Enrich.FromLogContext()
            .Enrich.WithSpanTiming()
            .CreateLogger();

        builder.Host.UseSerilog();

        return services;
    }


}
