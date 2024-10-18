using Microsoft.Extensions.DependencyInjection.Extensions;
using Noizera.Api.Common;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Noizera.Api;

internal static class DI
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, [NotNull] Assembly assembly)
    {
        var endpoints = assembly
            .DefinedTypes
            .Where(type => typeof(IEndpoint).IsAssignableFrom(type) && type.IsClass)
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type)).ToArray();

        services.TryAddEnumerable(endpoints);

        return services;
    }

    public static IApplicationBuilder UseEndpoints(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(app));
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        foreach (var endpoint in endpoints)
        {
            endpoint.Setup(app);
        }

        return app;
    }
}
