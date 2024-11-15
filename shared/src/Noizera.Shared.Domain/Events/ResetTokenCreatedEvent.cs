using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.Events;

public sealed record ResetTokenCreatedEvent(string Email, Guid UserId, string Token) : DomainEvent;