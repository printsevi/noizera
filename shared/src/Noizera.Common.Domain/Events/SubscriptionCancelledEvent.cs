using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.Events;

public sealed record SubscriptionCancelledEvent(Guid UserSubscriptionId) : DomainEvent;
