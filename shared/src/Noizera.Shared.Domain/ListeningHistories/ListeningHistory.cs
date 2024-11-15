using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Songs;
using Noizera.Common.Domain.Users;

namespace Noizera.Common.Domain.ListeningHistories;

public class ListeningHistory : BaseEntity
{
    public Guid ListenerUserId { get; private set; }
    public User Listener { get; } = null!;
    public Guid SongId { get; private set; }
    public Song Song { get; } = null!;
    public int ListeningTimeInSeconds { get; private set; }
    public int PlayCount { get; private set; }
    public DateTimeOffset? FirstPlayed { get; private set; }
    public DateTimeOffset? LastPlayed { get; private set; }
    public int StreamCount { get; private set; }
    public DateTimeOffset? FirstStreamed { get; private set; }
    public DateTimeOffset? LastStreamed { get; private set; }
    public int SkipCount { get; private set; }
    public DateTimeOffset? FirstSkipped { get; private set; }
    public DateTimeOffset? LastSkipped { get; private set; }

    private ListeningHistory(
        Guid listenerId,
        Guid songId,
        DateTimeOffset dateTime)
        : this(listenerId, songId)
    {
        PlayCount = 1;
        FirstPlayed = dateTime;
        LastPlayed = dateTime;
    }

    private ListeningHistory(
        Guid listenerId,
        Guid songId)
    {
        ListenerUserId = listenerId;
        SongId = songId;
    }

    public static ListeningHistory NewPlay(Guid listenerUserId, Guid songId)
    {
        var dateTime = SystemClock.UtcNow;
        return new ListeningHistory(listenerUserId, songId, dateTime);
    }

    public void AddStream(int listeningTimeInSeconds)
    {
        ListeningTimeInSeconds += listeningTimeInSeconds;
        StreamCount++;
        LastStreamed = SystemClock.UtcNow;
    }

    public void AddSkip()
    {
        SkipCount++;
        LastSkipped = SystemClock.UtcNow;
    }

    public void AddPlay()
    {
        PlayCount++;
        LastPlayed = SystemClock.UtcNow;
    }

    private ListeningHistory() { }
}
