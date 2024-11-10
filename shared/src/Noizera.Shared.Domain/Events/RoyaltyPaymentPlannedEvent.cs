using Noizera.Shared.Domain.Common;

namespace Noizera.Shared.Domain.Events;

public sealed record RoyaltyPaymentPlannedEvent(
    Guid UserId,
    DateTimeOffset EffectiveDate,
    DateTimeOffset EndDate,
    float RoyaltyShare,
    decimal SubscriptionPrice) : DomainEvent
{
    public override DateTimeOffset ProcessAfter { get; protected init; } = EndDate;
}
