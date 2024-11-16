using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.Events;

public sealed record AlbumReleasedEvent(Guid AlbumId) : DomainEvent
{
    public override bool RealTime { get; protected init; }
}
