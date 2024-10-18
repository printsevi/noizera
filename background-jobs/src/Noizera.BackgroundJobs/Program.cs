using Noizera.BackgroundJobs;
using Noizera.Shared.Infrastructure;
using Noizera.Shared.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSharedInfrastructure(builder)
    .AddServices();

var app = builder.Build();

app.MapEndpoints();

app.Run();
