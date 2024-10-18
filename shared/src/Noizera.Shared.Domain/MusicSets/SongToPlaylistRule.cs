using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.Songs;

namespace Noizera.Shared.Domain.MusicSets;

public sealed record SongToPlaylistRule(Playlist playlist, Song newSong) : ISyncDomainRule
{
    public string ErrorMessage => $"The song is already in playlist.";

    public bool Verify() => !playlist.MusicCollectionSongs.Any(x => x.Song.Id == newSong.Id);
}
