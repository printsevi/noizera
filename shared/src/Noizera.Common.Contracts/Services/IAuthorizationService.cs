using Noizera.Common.Contracts.Security;

namespace Noizera.Common.Contracts.Services;

public interface IAuthorizationService
{
    void AuthorizeCurrentUser<T>(
        IAuthorizeableRequest<T> request,
        IEnumerable<string> requiredRoles,
        IEnumerable<string> requiredPolicies);
}