using Dashboard.Jobs;
using Dashboard.Services.Subscriptions;
using Hangfire;
using Hangfire.PostgreSql;

namespace Dashboard;

public static class DI
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<SubscriptionService>();

        services.AddScoped<ProcessSongSimilaritiesJob>();
        services.AddScoped<ProcessSongPreferencesJob>();
    }

    public static void AddHangfire(this IServiceCollection services, string? connectionString)
    {
        services.AddHangfire(configuration =>
        {
            configuration.UsePostgreSqlStorage(o => o.UseNpgsqlConnection(connectionString));
        });

        services.AddHangfireServer(opt => opt.SchedulePollingInterval = TimeSpan.FromSeconds(1));
    }

    public static void UseHangfire(this WebApplication app)
    {
        app.UseHangfireDashboard(options: new DashboardOptions
        {
            DarkModeEnabled = true,
            Authorization = []
        });

        //app.Services
        //    .GetRequiredService<IRecurringJobManager>()
        //    .AddOrUpdate<IProcessSongSimilaritiesJob>(
        //        "song-similarities",
        //        job => job.ProcessAsync(),
        //        app.Configuration["Jobs:SongSimilarities:Schedule"]);
    }
}
