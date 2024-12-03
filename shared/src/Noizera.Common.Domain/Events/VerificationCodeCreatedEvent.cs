using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.Events;

public sealed record VerificationCodeCreatedEvent(string Email, string Name, string Code) : DomainEvent;
