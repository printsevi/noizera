namespace Noizera.Common.Domain.Profiles;

public interface IProfileUniquenessChecker
{
    Task<bool> VerifyUsernameAsync(string username, CancellationToken ct = default);
}