using Noizera.Shared.Domain.Common;

namespace Noizera.Shared.Domain.Users;

public sealed record UpdateProfileTypeRule(User User) : ISyncDomainRule
{
    public string ErrorMessage => $"The profile type can't be updated because the user have songs released.";

    public bool Verify() => User.Songs.Count == 0;
}
