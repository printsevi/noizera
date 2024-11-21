using Microsoft.EntityFrameworkCore;
using Noizera.Api;
using Noizera.Api.Middlewares;
using Noizera.Application;
using Noizera.Common.Persistence.SQL;
using Noizera.Infrastructure;
using System.Reflection;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services
    .AddAuthorization()
    .AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN")
    .AddEndpoints(Assembly.Load("Noizera.Api"))
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder)
    .AddMemoryCache();

string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

string[]? frontendUrls = builder.Configuration.GetSection("FrontendUrls").Get<string[]>();
builder.Services.AddCors(options => options.AddPolicy(name: MyAllowSpecificOrigins, policy => policy.WithOrigins(frontendUrls!)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>(); //Always FIRST
app.UseCors(MyAllowSpecificOrigins);
app.UseMiddleware<RateLimitingMiddleware>();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints();

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
