using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.Users;

public sealed record UserEmailUniquenessRule(string Email, IUserUniquenessChecker Checker) : IAsyncDomainRule
{
    public string ErrorMessage => $"Email {Email} is already in use.";

    public async Task<bool> VerifyAsync(CancellationToken ct = default)
        => await Checker.VerifyEmailAsync(Email, ct).ConfigureAwait(false);
}
