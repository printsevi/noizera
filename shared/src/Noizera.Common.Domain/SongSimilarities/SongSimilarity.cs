using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.SongSimilarities;

public sealed class SongSimilarity : BaseEntity
{
    public Guid FirstSongId { get; private set; }
    public Guid SecondSongId { get; private set; }
    public float? Value { get; private set; }
    public DateTimeOffset LastProcessedOn { get; private set; }

    private SongSimilarity(Guid firstSongId, Guid secondSongId, float score)
        : base()
    {
        FirstSongId = firstSongId;
        SecondSongId = secondSongId;
        Value = score;
    }

    public static SongSimilarity Create(Guid firstSongId, Guid secondSongId, float score)
    {
        return new SongSimilarity(firstSongId, secondSongId, score);
    }


    private SongSimilarity() { }
}
