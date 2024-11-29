using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.MusicSets;
using Noizera.Common.Domain.Users;

namespace Noizera.Common.Domain.SavedMusicSets;

public sealed record MusicSetOwnerRule(User User, MusicSet MusicSet) : ISyncDomainRule
{
    public string ErrorMessage => $"The playlist can't be saved by the owner.";

    public bool Verify() => MusicSet is not Playlist playlist || playlist.OwnerId != User.Id;
}
