using Microsoft.AspNetCore.Http;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Security;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Noizera.Infrastructure.Security.UserProviders;

public class CurrentUserCustomProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    public CurrentUser GetCurrentUser()
    {
        var principal = httpContextAccessor.HttpContext?.User
            ?? throw new AppException("No authenticated request context.", ErrorType.Authorization);
        string? userIdValue = principal.FindFirst(ClaimType.UserId)?.Value;
        if (!Guid.TryParse(userIdValue, CultureInfo.InvariantCulture, out var userId))
        {
            throw new AppException("The access token does not identify a user.", ErrorType.Authorization);
        }

        var roles = principal.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToList();
        string email = principal.FindFirst(ClaimTypes.Email)?.Value
            ?? principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value
            ?? string.Empty;
        string username = principal.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value
            ?? principal.FindFirst(ClaimTypes.Name)?.Value
            ?? string.Empty;

        return new(userId, email, username, roles);
    }
}
