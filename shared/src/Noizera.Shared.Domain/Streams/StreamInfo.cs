using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Songs;
using Noizera.Common.Domain.Users;

namespace Noizera.Common.Domain.Streams;

public class StreamInfo : Entity
{
    public Guid UserId { get; private set; }
    public User User { get; } = null!;
    public Guid SongId { get; private set; }
    public Song Song { get; } = null!;
    public int TimeInSeconds { get; private set; }

    private StreamInfo(
        Guid userId,
        Guid songId,
        int timeInSeconds)
        : this()
    {
        UserId = userId;
        SongId = songId;
        TimeInSeconds = timeInSeconds;
    }

    public static StreamInfo New(Guid userId, Guid songId, int timeInSeconds)
    {
        return new(userId, songId, timeInSeconds);
    }

    private StreamInfo() { }
}
