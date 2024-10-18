namespace Noizera.Infrastructure.Security.UserProviders;

public record CurrentUser(
    Guid UserId,
    string Email,
    string Username,
    IReadOnlyList<string> Roles);