using Noizera.Infrastructure.Security.UserProviders;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Infrastructure.Security.Policy;

public class PolicyEnforcer : IPolicyEnforcer
{
    public void Authorize<T>(
        [NotNull] IAuthorizeableRequest<T> request,
        [NotNull] CurrentUser currentUser,
        string policy) => _ = policy switch
        {
            PolicyTypes.SelfOrAdmin => SelfOrAdminPolicy(request, currentUser),
            _ => throw new AppException("Unknown policy name", ErrorType.Authorization),
        };

    private static bool SelfOrAdminPolicy<T>(IAuthorizeableRequest<T> request, CurrentUser currentUser) => request.UserId != currentUser.UserId && !currentUser.Roles.Contains(Role.Admin)
            ? throw new AppException("Requesting user failed policy requirement", ErrorType.Authorization)
            : true;
}