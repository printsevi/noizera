using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.Events;

public sealed record RoyaltyPaymentPlannedEvent(
    Guid UserId,
    DateTimeOffset EffectiveDate,
    DateTimeOffset EndDate,
    float RoyaltyShare,
    decimal SubscriptionPrice) : DomainEvent
{
    public override DateTimeOffset ProcessAfter { get; protected init; } = EndDate;
}
