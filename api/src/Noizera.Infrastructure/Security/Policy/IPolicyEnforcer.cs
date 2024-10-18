using Noizera.Infrastructure.Security.UserProviders;
using Noizera.Shared.Contracts.Security;

namespace Noizera.Infrastructure.Security.Policy;

public interface IPolicyEnforcer
{
    public void Authorize<T>(IAuthorizeableRequest<T> request, CurrentUser currentUser, string policy);
}