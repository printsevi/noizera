namespace Noizera.Shared.Domain.Profiles;

public interface IProfileUniquenessChecker
{
    Task<bool> VerifyUsernameAsync(string username, CancellationToken ct = default);
}