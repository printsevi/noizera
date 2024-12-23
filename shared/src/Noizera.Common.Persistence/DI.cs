using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Noizera.Common.Persistence.S3;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Common.Persistence;

public static class DI
{
    public static IServiceCollection AddSharedPersistence(
        this IServiceCollection services,
        [NotNull] IConfiguration configuration,
        bool efTrackingDisabled = true)
    {
        services
            .AddSqlDb(configuration, efTrackingDisabled)
            .AddS3(configuration)
            .AddSharedHealthChecks(configuration);

        return services;
    }

    private static IServiceCollection AddSqlDb(this IServiceCollection services, [NotNull] IConfiguration configuration, bool trackingDisabled = true)
    {
        _ = services.AddDbContext<AppDbContext>(opts =>
        {
            _ = opts.UseNpgsql(configuration.GetConnectionString("PostgresConnection"));
            _ = opts.UseQueryTrackingBehavior(trackingDisabled ? QueryTrackingBehavior.NoTracking : QueryTrackingBehavior.TrackAll);
        });

        return services;
    }

    private static IServiceCollection AddS3(this IServiceCollection services, [NotNull] IConfiguration configuration)
    {
        _ = services.Configure<S3BucketSettings>(configuration.GetSection("S3Bucket"));
        var s3Settings = configuration.GetSection("S3").Get<S3Settings>()!;

        _ = services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(s3Settings.SpacesKey, s3Settings.SpacesSecret, new AmazonS3Config
        {
            ServiceURL = s3Settings.ServiceUrl.ToString(),
            ForcePathStyle = true,
            Timeout = TimeSpan.FromSeconds(30)
        }));

        _ = services.AddSingleton<S3Context>();

        return services;
    }

    public static void AddSharedHealthChecks(this IServiceCollection services, [NotNull] IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("PostgresConnection")!);
    }
}
