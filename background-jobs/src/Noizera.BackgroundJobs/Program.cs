using Noizera.BackgroundJobs;
using Noizera.Shared.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSharedInfrastructure(builder)
    .AddServices(builder.Configuration);

var app = builder.Build();

app.MapEndpoints();

app.Run();