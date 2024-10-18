using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Noizera.Shared.Persistence.SQL;
using Npgsql;
using System.Data;
using System.Data.Common;
using Testcontainers.PostgreSql;

namespace Noizera.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Local");
        builder.ConfigureServices(services =>
        {
            services.Remove(services.SingleOrDefault(service => typeof(DbContextOptions<AppDbContext>) == service.ServiceType)!);
            services.Remove(services.SingleOrDefault(service => typeof(DbConnection) == service.ServiceType)!);
            services.Remove(services.SingleOrDefault(service => typeof(IDbConnection) == service.ServiceType)!);
            services.AddTransient<IDbConnection>(db => new NpgsqlConnection(_postgreSqlContainer.GetConnectionString()));
            services.AddDbContext<AppDbContext>((_, option) => option.UseNpgsql(_postgreSqlContainer.GetConnectionString()));
        });
    }

    public Task InitializeAsync() => _postgreSqlContainer.StartAsync();

    public new Task DisposeAsync() => _postgreSqlContainer.DisposeAsync().AsTask();
}
