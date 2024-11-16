namespace Noizera.Common.Domain.Users;

public interface IUserUniquenessChecker
{
    Task<bool> VerifyEmailAsync(string email, CancellationToken ct = default);
}