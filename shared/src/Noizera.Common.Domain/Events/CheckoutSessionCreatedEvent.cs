using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.Events;

public sealed record CheckoutSessionCreatedEvent(Guid UserSubscriptionId, string StripeSessionId) : DomainEvent;
