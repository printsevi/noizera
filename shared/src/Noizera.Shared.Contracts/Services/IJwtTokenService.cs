using Noizera.Common.Domain.Users;

namespace Noizera.Common.Contracts.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);

    Guid? GetUserId(string expiredAccessToken);
}