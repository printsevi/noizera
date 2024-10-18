using Microsoft.EntityFrameworkCore;
using Noizera.Api;
using Noizera.Api.Middlewares;
using Noizera.Application;
using Noizera.Infrastructure;
using Noizera.Shared.Persistence.SQL;
using Serilog;
using SerilogTracing;
using System.Reflection;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services
    .AddAuthorization()
    .AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN")
    .AddEndpoints(Assembly.GetExecutingAssembly())
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder);

string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options => options.AddPolicy(name: MyAllowSpecificOrigins, policy => policy.WithOrigins(builder.Configuration["FrontendUrl"]!)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));

if (builder.Environment.IsProduction())
{
    _ = builder.WebHost.UseKestrelHttpsConfiguration();
}

var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints();

if (app.Environment.IsProduction())
{
    _ = app.UseHttpsRedirection();
}

if (!builder.Environment.EnvironmentName.Equals("Local"))
{
    _ = app.MapHealthChecks("/api/health");
}

using var scope = app.Services.CreateScope();
using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await dbContext.Database.MigrateAsync().ConfigureAwait(false);

await app.RunAsync().ConfigureAwait(false);

//Configure integration tests
public partial class Program { }
