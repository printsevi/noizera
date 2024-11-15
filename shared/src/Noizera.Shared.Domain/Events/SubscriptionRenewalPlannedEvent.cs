using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.Events;

public sealed record SubscriptionRenewalPlannedEvent(
    Guid UserSubscriptionId,
    DateTimeOffset EndDate) : DomainEvent
{
    public override DateTimeOffset ProcessAfter { get; protected init; } = EndDate;
}
