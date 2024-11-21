using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.Users;

public sealed record UpdateProfileTypeRule(User User) : ISyncDomainRule
{
    public string ErrorMessage => $"The profile type can't be updated because the user have songs released.";

    public bool Verify() => !User.Songs.Any(x => x.IsPublic);
}
