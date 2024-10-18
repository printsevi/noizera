namespace Noizera.Infrastructure.Security.UserProviders;

public interface ICurrentUserProvider
{
    CurrentUser GetCurrentUser();
}