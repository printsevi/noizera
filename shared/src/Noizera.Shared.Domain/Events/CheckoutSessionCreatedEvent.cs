using Noizera.Shared.Domain.Common;

namespace Noizera.Shared.Domain.Events;

public sealed record CheckoutSessionCreatedEvent(Guid UserSubscriptionId, string StripeSessionId) : DomainEvent;
