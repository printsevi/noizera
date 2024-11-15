namespace Noizera.Common.Domain.Common;

public abstract record DomainEvent
{
    public DateTimeOffset OccurredOn { get; private init; } = SystemClock.UtcNow;
    public virtual DateTimeOffset ProcessAfter { get; protected init; } = SystemClock.UtcNow;
    public virtual bool RealTime { get; protected init; } = true;
}
