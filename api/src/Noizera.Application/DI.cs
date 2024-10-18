using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Noizera.Application.Behaviors;
using System.Reflection;

namespace Noizera.Application;

public static class DI
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddMediatR(options =>
        {
            _ = options.RegisterServicesFromAssembly(typeof(DI).Assembly);
            _ = options.AddOpenBehavior(typeof(ExceptionPipelineBehavior<,>));
            _ = options.AddOpenBehavior(typeof(LoggingPipelineBehavior<,>));
            _ = options.AddOpenBehavior(typeof(AuthorizationPipelineBehavior<,>));
            _ = options.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        _ = services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
