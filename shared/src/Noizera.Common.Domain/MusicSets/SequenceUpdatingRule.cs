using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.MusicSetSongs;

namespace Noizera.Common.Domain.MusicSets;

public sealed record SequenceUpdatingRule(MusicSetSong? ActiveSong, MusicSetSong? OverSong) : ISyncDomainRule
{
    public string ErrorMessage => $"Sequence update failed";

    public bool Verify() => ActiveSong is not null && OverSong is not null && ActiveSong.Id != OverSong.Id;
}
