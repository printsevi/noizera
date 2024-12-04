using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.Events;

public sealed record PasswordUpdatedEvent(Guid UserId) : DomainEvent;
