using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.Events;

public sealed record UserSubscriptionActivatedEvent(Guid UserSubscriptionId) : DomainEvent;
