using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.MusicSetSongs;
using Noizera.Common.Domain.Songs;
using Noizera.Common.Domain.Users;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Common.Domain.MusicSets;

public sealed class Playlist : MusicSet
{
    public override string PublicIdPrefix => "p-";

    public string? PlaylistTag { get; private set; }

    public bool IsPublicPlaylist { get; private set; }

    private Playlist(User user, string title, string publicId, string? playlistTag = null, bool isPublic = false) : base(user, publicId, title)
    {
        PlaylistTag = playlistTag;
        IsPublicPlaylist = isPublic;
    }

    public static async Task<Playlist> NewFavouritesAsync(User user, [NotNull] IHashGenerator hashGenerator, CancellationToken ct)
    {
        string publicId = await hashGenerator.GenerateAsync(ct).ConfigureAwait(false);
        Playlist playlist = new(user, PlaylistConstants.FavouritesPlaylistTitle, publicId, PlaylistConstants.FavouritesPlaylistTag);

        return playlist;
    }

    public void AddSong(Song song)
    {
        EnsureRule(new SongToPlaylistRule(this, song));

        short maxSequence = MusicSetSongs.Count > 0 ? MusicSetSongs.Max(x => x.Sequence) : (short)0;
        MusicSetSong musicSetSong = MusicSetSong.Create(song, this, ++maxSequence);
        MusicSetSongs.Add(musicSetSong);
    }

    private Playlist() { }
}
