using Noizera.Shared.Domain.Common;

namespace Noizera.Shared.Domain.Events;

public sealed record PasswordUpdatedEvent(Guid userId) : DomainEvent;
