using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Profiles;
using Noizera.Common.Domain.Users;

namespace Noizera.Common.Domain.MusicSets;

public sealed record AlbumCreationRule(User user) : ISyncDomainRule
{
    public string ErrorMessage => $"Album Creation failed. Wrong Profile type.";

    public bool Verify() => user.Profile?.ProfileType is ProfileType.Artist or ProfileType.Label;
}
