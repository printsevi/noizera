using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.Events;

public sealed record ResetTokenCreatedEvent(Guid UserId, Uri Link) : DomainEvent;