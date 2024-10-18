using Noizera.Shared.Contracts.Security;

namespace Noizera.Shared.Contracts.Services;

public interface IAuthorizationService
{
    void AuthorizeCurrentUser<T>(
        IAuthorizeableRequest<T> request,
        IEnumerable<string> requiredRoles,
        IEnumerable<string> requiredPolicies);
}