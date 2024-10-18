using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.MusicSets;
using Noizera.Shared.Domain.Users;

namespace Noizera.Shared.Domain.Songs;

public sealed record AlbumOwnerSongRule(User User, Album Album) : ISyncDomainRule
{
    public string ErrorMessage => $"The action can be performed only by the album creator.";

    public bool Verify() => Album.OwnerId == User.Id;
}
