using Noizera.Shared.Domain.Common;

namespace Noizera.Shared.Domain.Events;

public sealed record SubscriptionRenewalPlannedEvent(
    Guid UserSubscriptionId, 
    DateTimeOffset EndDate) : DomainEvent
{
    public override DateTimeOffset ProcessAfter { get; protected init; } = EndDate;
}
