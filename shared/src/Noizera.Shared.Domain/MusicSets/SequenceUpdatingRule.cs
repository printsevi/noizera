using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.MusicSetSongs;

namespace Noizera.Shared.Domain.MusicSets;

public sealed record SequenceUpdatingRule(MusicSetSong? ActiveSong, MusicSetSong? OverSong) : ISyncDomainRule
{
    public string ErrorMessage => $"Sequence update failed";

    public bool Verify() => ActiveSong is not null && OverSong is not null && ActiveSong.Id != OverSong.Id;
}
