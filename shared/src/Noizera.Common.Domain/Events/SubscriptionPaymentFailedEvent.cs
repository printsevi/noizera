using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.Events;

public sealed record SubscriptionPaymentFailedEvent(Guid UserSubscriptionId) : DomainEvent;
