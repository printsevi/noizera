using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Noizera.Shared.Persistence.S3;
using Noizera.Shared.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Shared.Persistence;

public static class DI
{
    public static IServiceCollection AddSharedPersistence(
        this IServiceCollection services,
        [NotNull] IConfiguration configuration)
    {
        services
            .AddSqlDb(configuration)
            .AddS3(configuration)
            .AddSharedHealthChecks(configuration);

        return services;
    }

    private static IServiceCollection AddSqlDb(this IServiceCollection services, [NotNull] IConfiguration configuration)
    {
        _ = services.AddDbContext<AppDbContext>(opts =>
        {
            _ = opts.UseNpgsql(configuration.GetConnectionString("PostgresConnection"));
            _ = opts.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        return services;
    }

    private static IServiceCollection AddS3(this IServiceCollection services, [NotNull] IConfiguration configuration)
    {
        _ = services.Configure<S3BucketSettings>(configuration.GetSection("S3Buckets"));
        var s3Settings = configuration.GetSection("S3").Get<S3Settings>()!;

        services.AddSingleton<IAmazonS3>(sp =>
        {
            return new AmazonS3Client(s3Settings.SpacesKey, s3Settings.SpacesSecret, new AmazonS3Config
            {
                ServiceURL = s3Settings.ServiceUrl,
                ForcePathStyle = true
            });
        });

        services.AddSingleton<S3Context>();

        return services;
    }

    public static void AddSharedHealthChecks(this IServiceCollection services, [NotNull] IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("PostgresConnection")!);
    }
}
