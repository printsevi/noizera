using Noizera.Shared.Domain.Common;

namespace Noizera.Shared.Domain.Events;

public sealed record ResetTokenCreatedEvent(string Email, Guid UserId, string Token) : DomainEvent;