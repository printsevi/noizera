using Noizera.BackgroundJobs;
using Noizera.Common.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSharedInfrastructure(builder, efTrackingDisabled: false)
    .AddServices(builder.Configuration);

var app = builder.Build();

app.MapEndpoints();

app.Run();
