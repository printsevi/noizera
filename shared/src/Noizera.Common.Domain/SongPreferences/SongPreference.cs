using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.SongPreferences;

public sealed class SongPreference : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid SongId { get; private set; }
    public float? Value { get; private set; }
    public DateTimeOffset LastProcessedOn { get; private set; }

    private SongPreference(Guid userId, Guid songId, float score)
        : base()
    {
        UserId = userId;
        SongId = songId;
        Value = score;
    }

    public void Process(float score)
    {
        Value = score;
        LastProcessedOn = SystemClock.UtcNow;
    }

    public static SongPreference Create(Guid firstSongId, Guid secondSongId, float score)
    {
        return new SongPreference(firstSongId, secondSongId, score);
    }

    private SongPreference() { }
}
