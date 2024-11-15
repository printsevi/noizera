using Noizera.Infrastructure.Security.Policy;
using Noizera.Infrastructure.Security.UserProviders;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Contracts.Services;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Infrastructure.Security.Authorization;

public class AuthorizationService(IPolicyEnforcer policyEnforcer, ICurrentUserProvider currentUserProvider) : IAuthorizationService
{
    public void AuthorizeCurrentUser<T>(
        IAuthorizeableRequest<T> request,
        [NotNull] IEnumerable<string> requiredRoles,
        [NotNull] IEnumerable<string> requiredPolicies)
    {
        var currentUser = currentUserProvider.GetCurrentUser();

        if (requiredRoles.Except(currentUser.Roles).Any())
        {
            throw new AppException("User is missing required roles for taking this action", ErrorType.Authorization);
        }

        foreach (string policy in requiredPolicies)
        {
            policyEnforcer.Authorize(request, currentUser, policy);
        }
    }
}