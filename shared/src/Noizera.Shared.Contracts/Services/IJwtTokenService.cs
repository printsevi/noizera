using Noizera.Shared.Domain.Users;

namespace Noizera.Shared.Contracts.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);

    Guid? GetUserId(string expiredAccessToken);
}