using Noizera.BackgroundJobs;
using Noizera.Common.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSharedInfrastructure(builder)
    .AddServices(builder.Configuration);

var app = builder.Build();

app.MapEndpoints();

app.Run();
