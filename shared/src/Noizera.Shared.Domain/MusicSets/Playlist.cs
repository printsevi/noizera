using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.MusicCollectionSongs;
using Noizera.Shared.Domain.Songs;
using Noizera.Shared.Domain.Users;

namespace Noizera.Shared.Domain.MusicSets;

public sealed class Playlist : MusicSet
{
    public override string PublicIdPrefix => "p-";

    public string? PlaylistTag { get; private set; }

    private Playlist(User user, string title, string? playlistTag = null, string? publicId = null) : base(user, title)
    {
        PlaylistTag = playlistTag;
        SetPublicId(publicId);
    }

    public static Playlist New(User user, string title)
    {
        Playlist playlist = new(user, title);

        return playlist;
    }

    public static async Task<Playlist> NewFavouritesAsync(User user, IHashGenerator hashGenerator, CancellationToken ct)
    {
        var publicId = await hashGenerator.GenerateAsync(ct);
        Playlist playlist = new(user, PlaylistConstants.FavouritesPlaylistTitle, PlaylistConstants.FavouritesPlaylistTag, publicId);

        return playlist;
    }

    public void AddSong(Song song)
    {
        EnsureRule(new SongToPlaylistRule(this, song));

        short maxSequence = MusicCollectionSongs.Count > 0 ? MusicCollectionSongs.Max(x => x.Sequence) : (short)0;
        var musicCollectionSong = MusicCollectionSong.Create(song, this, ++maxSequence);
        MusicCollectionSongs.Add(musicCollectionSong);
    }

    private Playlist() { }
}
