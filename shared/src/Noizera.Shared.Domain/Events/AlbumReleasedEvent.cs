using Noizera.Shared.Domain.Common;

namespace Noizera.Shared.Domain.Events;

public sealed record AlbumReleasedEvent(Guid AlbumId) : DomainEvent
{
    public override bool RealTime { get; protected init; } = false;
}
