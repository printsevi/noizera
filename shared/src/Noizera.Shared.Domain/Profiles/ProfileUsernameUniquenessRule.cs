using Noizera.Shared.Domain.Common;

namespace Noizera.Shared.Domain.Profiles;

public sealed record ProfileUsernameUniquenessRule(string Username, IProfileUniquenessChecker Checker) : IAsyncDomainRule
{
    public string ErrorMessage => $"Username {Username} is already in use.";

    public async Task<bool> VerifyAsync(CancellationToken ct = default)
        => await Checker.VerifyUsernameAsync(Username, ct).ConfigureAwait(false);
}
