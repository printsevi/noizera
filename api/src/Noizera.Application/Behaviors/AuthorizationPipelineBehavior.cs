using MediatR;
using Noizera.Shared.Contracts.Security;
using Noizera.Shared.Contracts.Services;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Noizera.Application.Behaviors;

public class AuthorizationPipelineBehavior<TRequest, TResponse>(
    IAuthorizationService authorizationService)
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IAuthorizeableRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, [NotNull] RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        List<AuthorizeAttribute> authorizationAttributes = request.GetType()
            .GetCustomAttributes<AuthorizeAttribute>()
            .ToList();

        if (authorizationAttributes.Count == 0)
        {
            return await next().ConfigureAwait(false);
        }

        var requiredPermissions = authorizationAttributes.SelectMany(authorizationAttribute => authorizationAttribute.Permissions?.Split(',') ?? []);
        var requiredRoles = authorizationAttributes.SelectMany(authorizationAttribute => authorizationAttribute.Roles?.Split(',') ?? []);
        var requiredPolicies = authorizationAttributes.SelectMany(authorizationAttribute => authorizationAttribute.Policies?.Split(',') ?? []);
        authorizationService.AuthorizeCurrentUser(request, requiredRoles, requiredPolicies);

        return await next().ConfigureAwait(false);
    }
}