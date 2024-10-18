using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Noizera.Shared.Persistence.Mongo;
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
            .AddMongoDb(configuration)
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

    private static IServiceCollection AddMongoDb(this IServiceCollection services, [NotNull] IConfiguration configuration)
    {
        _ = services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

        _ = services.AddSingleton<MongoDbContext>();

        return services;
    }

    public static void AddSharedHealthChecks(this IServiceCollection services, [NotNull] IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("PostgresConnection")!)
            .AddMongoDb(configuration["MongoDbSettings:ConnectionString"]!);
    }
}
