using Noizera.Shared.Domain.Common;

namespace Noizera.Shared.Domain.MusicSets;

public sealed record AlbumReleaseRule(Album Album) : ISyncDomainRule
{
    public string ErrorMessage => $"Album {Album.Id} Release failed";

    public bool Verify() =>
        Album.AlbumStatus == AlbumStatus.Submitted
        && !Album.MusicSetSongs.Any(x => !x.HasAudioAttached);
}
