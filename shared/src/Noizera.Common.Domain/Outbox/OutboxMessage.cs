using Noizera.Common.Domain.Common;
using System.Linq.Expressions;

namespace Noizera.Common.Domain.Outbox;

public class OutboxMessage : Entity
{
    public string Type { get; private set; } = null!;
    public string Data { get; private set; } = null!;
    public DateTimeOffset OccurredOn { get; private set; }
    public DateTimeOffset ProcessAfter { get; private set; }
    public DateTimeOffset? ProcessedOn { get; private set; }
    public DateTimeOffset? FailedOn { get; private set; }
    public bool IsRealTime { get; private set; }
    public short Retries { get; private set; }
    public string? ErrorText { get; private set; }

    public static Expression<Func<OutboxMessage, bool>> IsProcessableExpression(bool isRealTime) =>
        message => message.ProcessedOn == null && SystemClock.UtcNow > message.ProcessAfter && message.IsRealTime == isRealTime;

    private OutboxMessage(
        DateTimeOffset occurredOn,
        string type,
        string data,
        bool isRealTime,
        DateTimeOffset processAfter) : base()
    {
        OccurredOn = occurredOn;
        Type = type;
        Data = data;
        IsRealTime = isRealTime;
        ProcessAfter = processAfter;
    }

    public static OutboxMessage New(
        DateTimeOffset occurredOn,
        string type,
        string data,
        bool isRealTime,
        DateTimeOffset processAfter)
    {
        return new(occurredOn, type, data, isRealTime, processAfter);
    }

    public void Process()
    {
        ProcessedOn = SystemClock.UtcNow;
    }

    public void Fail(string error)
    {
        FailedOn = SystemClock.UtcNow;
        ProcessAfter = SystemClock.UtcNow.AddSeconds(5);
        ErrorText = error;
        Retries++;
    }

    public void Postpone(int seconds)
    {
        ProcessAfter = SystemClock.UtcNow.AddSeconds(seconds);
    }

    private OutboxMessage() { }
}
