using Noizera.Shared.Domain.Common;

namespace Noizera.Shared.Domain.Events;

public sealed record AlbumSubmittedEvent(Guid AlbumId) : DomainEvent
{
    public override DateTimeOffset ProcessAfter { get; protected init; } = SystemClock.UtcNow.AddMinutes(1);
    public override bool RealTime { get; protected init; } = false;
}
