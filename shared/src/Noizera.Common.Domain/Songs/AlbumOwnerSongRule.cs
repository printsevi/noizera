using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.MusicSets;
using Noizera.Common.Domain.Users;

namespace Noizera.Common.Domain.Songs;

public sealed record AlbumOwnerSongRule(User User, Album Album) : ISyncDomainRule
{
    public string ErrorMessage => $"The action can be performed only by the album creator.";

    public bool Verify() => Album.OwnerId == User.Id;
}
