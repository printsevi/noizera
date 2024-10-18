using Noizera.Shared.Domain.Common;

namespace Noizera.Shared.Domain.Songs;

public sealed record OwnerSongRule(Song Song, Guid UserId) : ISyncDomainRule
{
    public string ErrorMessage => $"The action can be performed only by the song owner.";

    public bool Verify() => Song.OwnerId == UserId;
}
