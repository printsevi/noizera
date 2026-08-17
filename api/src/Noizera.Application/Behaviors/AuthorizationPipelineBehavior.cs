using MediatR;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Contracts.Services;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Noizera.Application.Behaviors;

/// <summary>
/// Authorizes every <see cref="IAuthorizeableRequest{T}"/> before it reaches its handler.
/// </summary>
/// <remarks>
/// This behavior is deliberately <b>fail-closed</b>. Every authorizeable request carries a
/// <c>UserId</c> that is bound from the route or query string, so it is attacker-controlled;
/// without an ownership check an authenticated caller could act as any other user.
/// <para>
/// Therefore <see cref="PolicyTypes.SelfOrAdmin"/> is applied to <i>every</i> request by
/// default, whether or not it declares an <see cref="AuthorizeAttribute"/>. A request that
/// legitimately acts on behalf of another user must say so explicitly by declaring its own
/// policy — silence is never interpreted as permission.
/// </para>
/// </remarks>
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

        var requiredRoles = authorizationAttributes.SelectMany(attribute => SplitClaims(attribute.Roles)).ToList();
        var requiredPolicies = authorizationAttributes.SelectMany(attribute => SplitClaims(attribute.Policies)).ToList();

        if (requiredPolicies.Count == 0)
        {
            requiredPolicies.Add(PolicyTypes.SelfOrAdmin);
        }

        authorizationService.AuthorizeCurrentUser(request, requiredRoles, requiredPolicies);

        return await next().ConfigureAwait(false);
    }

    private static string[] SplitClaims(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? []
            : value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
