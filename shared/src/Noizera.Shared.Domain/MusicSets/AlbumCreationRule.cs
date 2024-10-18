using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.Profiles;
using Noizera.Shared.Domain.Users;

namespace Noizera.Shared.Domain.MusicSets;

public sealed record AlbumCreationRule(User user) : ISyncDomainRule
{
    public string ErrorMessage => $"Album Creation failed. Wrong Profile type.";

    public bool Verify() => user.Profile?.ProfileType is ProfileType.Artist or ProfileType.Label;
}
