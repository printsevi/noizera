using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.MusicSets;
using Noizera.Shared.Domain.Songs;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Shared.Domain.MusicCollectionSongs;

public sealed class MusicCollectionSong : Entity
{
    public Guid SongId { get; }
    public Song Song { get; } = null!;
    public Guid MusicCollectionId { get; }
    public MusicSet MusicCollection { get; } = null!;
    public short Sequence { get; private set; }

    public bool HasAudioAttached => Song.HasAudioAttached;

    private MusicCollectionSong(Song song, MusicSet musicCollection, short sequence)
        : base()
    {
        SongId = song.Id;
        MusicCollectionId = musicCollection.Id;
        Sequence = sequence;
    }

    public static MusicCollectionSong Create([NotNull] Song song, [NotNull] MusicSet musicCollection, short sequence)
        => new(song, musicCollection, sequence);

    public void SetSequence(short sequence) => Sequence = sequence;

    

    private MusicCollectionSong() { }
}
