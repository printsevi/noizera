using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Songs;

namespace Noizera.Common.Domain.MusicSets;

public sealed record SongToPlaylistRule(Playlist playlist, Song newSong) : ISyncDomainRule
{
    public string ErrorMessage => $"The song is already in playlist.";

    public bool Verify() => !playlist.MusicSetSongs.Any(x => x.Song.Id == newSong.Id);
}
