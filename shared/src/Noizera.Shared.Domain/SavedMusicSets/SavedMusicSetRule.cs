using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.MusicSets;
using Noizera.Shared.Domain.Users;

namespace Noizera.Shared.Domain.SavedMusicSets;

public sealed record SavedMusicSetRule(User User, MusicSet Collection) : ISyncDomainRule
{
    public string ErrorMessage => $"The collection is already in library.";

    public bool Verify() => !User.SavedMusicSets.Any(x => x.MusicSetId == Collection.Id);
}
