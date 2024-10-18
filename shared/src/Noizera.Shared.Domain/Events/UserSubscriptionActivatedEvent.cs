using Noizera.Shared.Domain.Common;

namespace Noizera.Shared.Domain.Events;

public sealed record UserSubscriptionActivatedEvent(Guid UserSubscriptionId) : DomainEvent;
