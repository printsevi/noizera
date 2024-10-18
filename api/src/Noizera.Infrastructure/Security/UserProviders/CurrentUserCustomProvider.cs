using Microsoft.AspNetCore.Http;
using Noizera.Shared.Contracts.Security;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Noizera.Infrastructure.Security.UserProviders;

public class CurrentUserCustomProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    public CurrentUser GetCurrentUser()
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

        Guid userId = Guid.Parse(GetSingleClaimValue(ClaimType.UserId), CultureInfo.InvariantCulture);
        var roles = GetClaimValues(ClaimTypes.Role);
        string username = GetSingleClaimValue(JwtRegisteredClaimNames.UniqueName);
        string email = GetSingleClaimValue(ClaimTypes.Email);

        return new(userId, email, username, roles);
    }

    private List<string> GetClaimValues(string claimType) =>
        httpContextAccessor.HttpContext!.User.Claims
            .Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value)
            .ToList();

    private string GetSingleClaimValue(string claimType) =>
        httpContextAccessor.HttpContext!.User.Claims
            .Single(claim => claim.Type == claimType)
            .Value;
}