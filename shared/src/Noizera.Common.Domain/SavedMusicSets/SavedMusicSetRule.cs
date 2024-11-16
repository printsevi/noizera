using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.MusicSets;
using Noizera.Common.Domain.Users;

namespace Noizera.Common.Domain.SavedMusicSets;

public sealed record SavedMusicSetRule(User User, MusicSet Collection) : ISyncDomainRule
{
    public string ErrorMessage => $"The collection is already in library.";

    public bool Verify() => !User.SavedMusicSets.Any(x => x.MusicSetId == Collection.Id);
}
