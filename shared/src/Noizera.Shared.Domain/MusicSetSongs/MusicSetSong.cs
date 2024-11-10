using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.MusicSets;
using Noizera.Shared.Domain.Songs;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Shared.Domain.MusicSetSongs;

public sealed class MusicSetSong : Entity
{
    public Guid SongId { get; }
    public Song Song { get; } = null!;
    public Guid MusicSetId { get; }
    public MusicSet MusicSet { get; } = null!;
    public short Sequence { get; private set; }

    public bool HasAudioAttached => Song.HasAudioAttached;

    private MusicSetSong(Song song, MusicSet MusicSet, short sequence)
        : base()
    {
        SongId = song.Id;
        MusicSetId = MusicSet.Id;
        Sequence = sequence;
    }

    public static MusicSetSong Create([NotNull] Song song, [NotNull] MusicSet MusicSet, short sequence)
        => new(song, MusicSet, sequence);

    public void SetSequence(short sequence) => Sequence = sequence;



    private MusicSetSong() { }
}
